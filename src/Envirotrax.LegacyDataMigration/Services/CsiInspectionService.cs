using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Csi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class CsiInspectionService
{
    private const string ScriptsFolder = "Scripts/CsiInspections";

    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<CsiInspectionService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public CsiInspectionService(
        ILogger<CsiInspectionService> logger,
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
        _logger.LogInformation("--------- Starting migration of CSI inspections ---------");

        await ExecuteSqlScriptsAsync();
        await MoveImagesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of CSI inspections ---------");
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
            _logger.LogInformation("Imported CSI inspections. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    private async Task MoveImagesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy CSI inspection images into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no CSI inspection image was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        var pendingImages = await _appDbContext
            .MigrationPendingCsiInspectionImages
            .AsNoTracking()
            .OrderBy(image => image.LegacyRecordId)
            .ToListAsync();

        _logger.LogInformation("Moving {Count} CSI inspection images.", pendingImages.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var pendingImage in pendingImages)
        {
            try
            {
                var download = await DownloadLegacyImageAsync(pendingImage);

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "Image {legacyRecordId} of CSI inspection {inspectionId} was not moved because {reason}. Legacy path: {legacyFilePath}.",
                        pendingImage.LegacyRecordId, pendingImage.InspectionId, download.UnavailableReason, pendingImage.LegacyFilePath);

                    unavailableCount++;
                    consecutiveFailureCount = 0;

                    continue;
                }

                var blobPath = $"professionals/{pendingImage.ProfessionalId}/csi-inspections/{pendingImage.InspectionId}/legacy-{pendingImage.LegacyRecordId}{Path.GetExtension(pendingImage.LegacyFilePath)}";

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);
                }

                await AddMigratedImageAsync(pendingImage, uploadedPath);

                movedCount++;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed moving image {legacyRecordId} of CSI inspection {inspectionId}. Error message: {errorMessage}.", pendingImage.LegacyRecordId, pendingImage.InspectionId, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} CSI inspection images failed in a row. The legacy file server or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} CSI inspection images, could not get {UnavailableCount} of them from V1, and failed {FailedCount} images.",
            movedCount, unavailableCount, failedCount);
    }

    private async Task<LegacyFileDownload> DownloadLegacyImageAsync(PendingCsiInspectionImage pendingImage)
    {
        if (!pendingImage.IsStoredInDatabase)
        {
            return await _legacyFileServerService.DownloadAsync(pendingImage.LegacyFilePath);
        }

        var imageData = await _appDbContext.Database
            .SqlQuery<byte[]>($"SELECT ImageData AS [Value] FROM Vepo.dbo.CsiInspectionImages WHERE ID = {pendingImage.LegacyRecordId}")
            .SingleOrDefaultAsync();

        if (imageData == null || imageData.Length == 0)
        {
            return new LegacyFileDownload { UnavailableReason = "the legacy database holds no image data for it" };
        }

        return new LegacyFileDownload { Content = new MemoryStream(imageData) };
    }

    private async Task AddMigratedImageAsync(PendingCsiInspectionImage pendingImage, string uploadedPath)
    {
        var image = new CsiInspectionImage
        {
            WaterSupplierId = pendingImage.WaterSupplierId,
            InspectionId = pendingImage.InspectionId,
            ProfessionalId = pendingImage.ProfessionalId,
            Description = pendingImage.Description,
            FilePath = uploadedPath,
            LegacyRecordId = pendingImage.LegacyRecordId
        };

        _appDbContext.CsiInspectionImages.Add(image);

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        finally
        {
            _appDbContext.Entry(image).State = EntityState.Detached;
        }
    }
}
