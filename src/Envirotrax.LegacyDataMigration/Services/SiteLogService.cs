
using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class SiteLogService
{
    private const string ScriptsFolder = "Scripts/SiteLogs";

    // The legacy file server or Azure Storage being down fails every row the same way. Stopping early
    // keeps a dead host from costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<SiteLogService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public SiteLogService(
        ILogger<SiteLogService> logger,
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
        _logger.LogInformation("--------- Starting migration of site logs ---------");

        await ExecuteSqlScriptsAsync();
        await MoveFileAttachmentsToAzureAsync();

        _logger.LogInformation("--------- Finished migration of site logs ---------");
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
            _logger.LogInformation("Imported site logs. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    // Copies every V1 attachment that has not been copied yet off the legacy file server and into
    // Azure Storage. FileAttachmentPath stays NULL until the file really is in Azure, so a log whose
    // file did not make it renders its file name as plain text instead of a link that goes nowhere.
    private async Task MoveFileAttachmentsToAzureAsync()
    {
        _logger.LogInformation("Moving legacy file attachments into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no attachment was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        // FileAttachmentPath is the completion flag: it is written only after the upload returned, so
        // anything still NULL has not made it to Azure yet and anything set is never touched again.
        // Read untracked, because tracking every pending row makes each save rescan all of them: that
        // is quadratic, and 20,000 rows spend about six minutes on change detection alone. Requiring a
        // LegacyRecordId keeps the blob name below from collapsing to a shared one if it is ever NULL.
        var pendingSiteLogs = await _appDbContext
            .SiteLogs
            .AsNoTracking()
            .Where(siteLog => siteLog.LegacyFilePath != null
                && siteLog.FileAttachmentPath == null
                && siteLog.LegacyRecordId != null)
            .OrderBy(siteLog => siteLog.Id)
            .ToListAsync();

        _logger.LogInformation("Moving attachments of {Count} site logs.", pendingSiteLogs.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var siteLog in pendingSiteLogs)
        {
            try
            {
                var download = await _legacyFileServerService.DownloadAsync(siteLog.LegacyFilePath!);

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "Attachment of site log {siteLogId} was not moved because {reason}. Legacy path: {legacyFilePath}.",
                        siteLog.Id, download.UnavailableReason, siteLog.LegacyFilePath);

                    unavailableCount++;

                    // The server answered, so it is alive. Only failures that look like an outage count
                    // towards stopping the run early.
                    consecutiveFailureCount = 0;

                    continue;
                }

                // Same folders as the V2 SiteLogService, with the V1 record id in place of a new GUID,
                // so a retry overwrites the same blob instead of orphaning one. The folder is the V2
                // SiteId, so that holds within one database only: rebuilding V2 from scratch reassigns
                // those ids, and the previous run's blobs are left behind rather than reused.
                var blobPath = $"site-logs/{siteLog.SiteId}/legacy-{siteLog.LegacyRecordId}{Path.GetExtension(siteLog.LegacyFilePath!)}";

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);
                }

                // Written one row at a time, after the upload, so a run that dies half way keeps every
                // file it already moved and never points a link at a blob that is not there.
                await _appDbContext
                    .SiteLogs
                    .Where(pendingSiteLog => pendingSiteLog.WaterSupplierId == siteLog.WaterSupplierId
                        && pendingSiteLog.Id == siteLog.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(pendingSiteLog => pendingSiteLog.FileAttachmentPath, uploadedPath)
                        .SetProperty(pendingSiteLog => pendingSiteLog.SkipFile, false));

                movedCount++;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed moving attachment of site log: {siteLogId}. Error message: {errorMessage}.", siteLog.Id, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} attachments failed in a row. The legacy file server or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} attachments, could not get {UnavailableCount} off the legacy file server, and failed {FailedCount} attachments.",
            movedCount, unavailableCount, failedCount);
    }
}
