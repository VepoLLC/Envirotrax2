
using System.Linq.Expressions;
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Fog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class FogInspectionService
{
    private const string ScriptsFolder = "Scripts/FogInspections";

    // The legacy file server or Azure Storage being down fails every image the same way. Stopping early
    // keeps a dead host from costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<FogInspectionService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public FogInspectionService(
        ILogger<FogInspectionService> logger,
        AppDbContext appDbContext,
        LegacyFileServerService legacyFileServerService,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _legacyFileServerService = legacyFileServerService;
        _blobStorageService = blobStorageService;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of FOG inspections ---------");

        await ExecuteSqlScriptsAsync();
        await MoveImagesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of FOG inspections ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", ScriptsFolder);

        var scripts = Directory.GetFiles(ScriptsFolder, "*.sql").OrderBy(file => file);

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _appDbContext.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Imported FOG inspections. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    // Copies every V1 exterior photo, interior photo and signature that has not been copied yet into
    // Azure Storage. Each image path stays NULL until that image really is in Azure, so an inspection
    // whose image did not make it simply shows none instead of a link that goes nowhere.
    private async Task MoveImagesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy FOG inspection images into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no FOG inspection image was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        // The image paths are the completion flags: each is written only after its upload returned, so
        // anything still NULL has not made it to Azure yet and anything set is never touched again.
        // Read untracked, because tracking every pending row makes each save rescan all of them.
        // Requiring a LegacyRecordId keeps the blob names below from collapsing to a shared one.
        var pendingInspections = await _appDbContext
            .FogInspections
            .AsNoTracking()
            .Where(inspection => inspection.LegacyRecordId != null
                && ((inspection.LegacyExteriorImagePath != null && inspection.ExteriorImagePath == null)
                    || (inspection.LegacyInteriorImagePath != null && inspection.InteriorImagePath == null)
                    || (inspection.HasLegacySignature == true && inspection.SignatureImagePath == null)))
            .OrderBy(inspection => inspection.Id)
            .ToListAsync();

        var pendingImages = pendingInspections
            .SelectMany(GetPendingImages)
            .ToList();

        _logger.LogInformation("Moving {Count} images of {InspectionCount} FOG inspections.", pendingImages.Count, pendingInspections.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var image in pendingImages)
        {
            var inspection = image.Inspection;

            try
            {
                var download = await image.DownloadAsync();

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "The {kind} image of FOG inspection {inspectionId} was not moved because {reason}.",
                        image.Kind, inspection.Id, download.UnavailableReason);

                    unavailableCount++;

                    // The source answered, so it is alive. Only failures that look like an outage count
                    // towards stopping the run early.
                    consecutiveFailureCount = 0;

                    continue;
                }

                // Same folders as the V2 FogInspectionService, with the V1 record id in place of a new
                // GUID, so a retry overwrites the same blob instead of orphaning one. The folder is the
                // V2 ProfessionalId, so that holds within one database only: rebuilding V2 from scratch
                // reassigns those ids, and the previous run's blobs are left behind rather than reused.
                var blobPath = $"professionals/{inspection.ProfessionalId}/fog-inspections/{image.Kind}/legacy-{inspection.LegacyRecordId}{image.Extension}";

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);
                }

                // Written one image at a time, after the upload, so a run that dies half way keeps every
                // image it already moved and never points a link at a blob that is not there.
                await _appDbContext
                    .FogInspections
                    .Where(pendingInspection => pendingInspection.WaterSupplierId == inspection.WaterSupplierId
                        && pendingInspection.Id == inspection.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(image.PathProperty, uploadedPath));

                movedCount++;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Failed moving the {kind} image of FOG inspection: {inspectionId}. Error message: {errorMessage}.",
                    image.Kind, inspection.Id, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} FOG inspection images failed in a row. The legacy file server, the legacy database or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} FOG inspection images, could not get {UnavailableCount} out of V1, and failed {FailedCount} images.",
            movedCount, unavailableCount, failedCount);
    }

    // The images of one inspection that are still waiting to be copied. The two photos are files on the
    // legacy server; the signature is read out of the legacy database instead.
    private IEnumerable<PendingImage> GetPendingImages(FogInspection inspection)
    {
        if (inspection.LegacyExteriorImagePath != null && inspection.ExteriorImagePath == null)
        {
            yield return new PendingImage(
                inspection,
                "exterior",
                Path.GetExtension(inspection.LegacyExteriorImagePath),
                () => _legacyFileServerService.DownloadAsync(inspection.LegacyExteriorImagePath),
                pendingInspection => pendingInspection.ExteriorImagePath);
        }

        if (inspection.LegacyInteriorImagePath != null && inspection.InteriorImagePath == null)
        {
            yield return new PendingImage(
                inspection,
                "interior",
                Path.GetExtension(inspection.LegacyInteriorImagePath),
                () => _legacyFileServerService.DownloadAsync(inspection.LegacyInteriorImagePath),
                pendingInspection => pendingInspection.InteriorImagePath);
        }

        if (inspection.HasLegacySignature == true && inspection.SignatureImagePath == null)
        {
            yield return new PendingImage(
                inspection,
                "signature",
                ".png",
                () => ReadLegacySignatureAsync(inspection.LegacyRecordId!.Value),
                pendingInspection => pendingInspection.SignatureImagePath);
        }
    }

    // V1 saves the signature pad as a PNG straight into FogInspections.Signature
    // (fog_inspectors/inspection_submit.aspx.vb, btnSignatureSave_Click). It is read one at a time, the
    // way a photo is downloaded, rather than all up front: holding every signature at once is what this
    // avoids, and the upload that follows costs far more than this lookup.
    private async Task<LegacyFileDownload> ReadLegacySignatureAsync(int legacyRecordId)
    {
        var signature = await _appDbContext
            .Database
            .SqlQuery<byte[]>($"""
                SELECT CAST(Signature AS VARBINARY(MAX)) AS Value
                FROM Vepo.dbo.FogInspections
                WHERE ID = {legacyRecordId}
                """)
            .FirstOrDefaultAsync();

        if (signature == null || signature.Length == 0)
        {
            return new LegacyFileDownload { UnavailableReason = "the legacy database no longer holds a signature for it" };
        }

        return new LegacyFileDownload { Content = new MemoryStream(signature) };
    }

    // Kind is the V2 folder the image goes into - exterior, interior or signature - the same names
    // Envirotrax.App's FogInspectionService uses. PathProperty is the column that is set once it is there.
    private record PendingImage(
        FogInspection Inspection,
        string Kind,
        string Extension,
        Func<Task<LegacyFileDownload>> DownloadAsync,
        Expression<Func<FogInspection, string?>> PathProperty);
}
