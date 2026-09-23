
using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class BackflowGaugeService
{
    private const string ScriptsFolder = "Scripts/BackflowGauges";

    // The legacy file server or Azure Storage being down fails every row the same way. Stopping early
    // keeps a dead host from costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<BackflowGaugeService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public BackflowGaugeService(
        ILogger<BackflowGaugeService> logger,
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
        _logger.LogInformation("--------- Starting migration of backflow gauges ---------");

        await ExecuteSqlScriptsAsync();
        await MoveGaugeFilesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of backflow gauges ---------");
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
            _logger.LogInformation("Imported backflow gauges. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    // Copies every V1 Test for Accuracy report that has not been copied yet off the legacy file server
    // and into Azure Storage. FilePath stays NULL until the file really is in Azure, so a gauge whose
    // report did not make it simply has no document instead of a link that goes nowhere.
    private async Task MoveGaugeFilesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy gauge files into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no gauge file was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        // FilePath is the completion flag: it is written only after the upload returned, so anything
        // still NULL has not made it to Azure yet and anything set is never touched again. Read
        // untracked, because tracking every pending row makes each save rescan all of them. Requiring a
        // LegacyRecordId keeps the blob name below from collapsing to a shared one if it is ever NULL.
        var pendingGauges = await _appDbContext
            .BackflowGauges
            .AsNoTracking()
            .Where(gauge => gauge.LegacyFilePath != null
                && gauge.FilePath == null
                && gauge.LegacyRecordId != null)
            .OrderBy(gauge => gauge.Id)
            .ToListAsync();

        _logger.LogInformation("Moving files of {Count} backflow gauges.", pendingGauges.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var gauge in pendingGauges)
        {
            try
            {
                var download = await _legacyFileServerService.DownloadAsync(gauge.LegacyFilePath!);

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "File of backflow gauge {gaugeId} was not moved because {reason}. Legacy path: {legacyFilePath}.",
                        gauge.Id, download.UnavailableReason, gauge.LegacyFilePath);

                    unavailableCount++;

                    // The server answered, so it is alive. Only failures that look like an outage count
                    // towards stopping the run early.
                    consecutiveFailureCount = 0;

                    continue;
                }

                // Same folders as the V2 BackflowGaugeService, with the V1 record id in place of a new
                // GUID, so a retry overwrites the same blob instead of orphaning one. The folder is the
                // V2 ProfessionalId, so that holds within one database only: rebuilding V2 from scratch
                // reassigns those ids, and the previous run's blobs are left behind rather than reused.
                var blobPath = $"professionals/{gauge.ProfessionalId}/gauges/legacy-{gauge.LegacyRecordId}{Path.GetExtension(gauge.LegacyFilePath!)}";

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);
                }

                // Written one row at a time, after the upload, so a run that dies half way keeps every
                // file it already moved and never points a link at a blob that is not there.
                await _appDbContext
                    .BackflowGauges
                    .Where(pendingGauge => pendingGauge.Id == gauge.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(pendingGauge => pendingGauge.FilePath, uploadedPath));

                movedCount++;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed moving file of backflow gauge: {gaugeId}. Error message: {errorMessage}.", gauge.Id, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} gauge files failed in a row. The legacy file server or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} gauge files, could not get {UnavailableCount} off the legacy file server, and failed {FailedCount} gauge files.",
            movedCount, unavailableCount, failedCount);
    }
}
