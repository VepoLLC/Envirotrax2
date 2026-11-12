using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Sites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class SiteLetterLogService
{
    private const string ScriptsFolder = "Scripts/SiteLetterLogs";

    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<SiteLetterLogService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyLetterFolderService _legacyLetterFolderService;
    private readonly BlobStorageService _blobStorageService;

    public SiteLetterLogService(
        ILogger<SiteLetterLogService> logger,
        AppDbContext appDbContext,
        LegacyLetterFolderService legacyLetterFolderService,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _legacyLetterFolderService = legacyLetterFolderService;
        _blobStorageService = blobStorageService;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of site letter logs ---------");

        await ExecuteSqlScriptsAsync();
        await MoveLettersToAzureAsync();

        _logger.LogInformation("--------- Finished migration of site letter logs ---------");
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
            _logger.LogInformation("Imported site letter logs. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    private async Task MoveLettersToAzureAsync()
    {
        _logger.LogInformation("Moving legacy letters into Azure Storage.");

        if (!Directory.Exists(_legacyLetterFolderService.LettersFolder))
        {
            _logger.LogError(
                "The legacy letters folder {lettersFolder} does not exist, so no letter was moved and every one of them is still pending. Set the letters folder in Program.cs, then run the migration again.",
                _legacyLetterFolderService.LettersFolder);

            return;
        }

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no letter was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        var legacyFilePaths = await _appDbContext
            .MigrationPendingSiteLetterLogs
            .AsNoTracking()
            .Select(letterLog => letterLog.LegacyFilePath)
            .Distinct()
            .OrderBy(legacyFilePath => legacyFilePath)
            .ToListAsync();

        _logger.LogInformation("Moving {Count} letter files.", legacyFilePaths.Count);

        var movedCount = 0;
        var letterCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var legacyFilePath in legacyFilePaths)
        {
            try
            {
                var download = await _legacyLetterFolderService.ReadAsync(legacyFilePath);

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "Letter file {legacyFilePath} was not moved because {reason}.",
                        legacyFilePath, download.UnavailableReason);

                    unavailableCount++;
                    consecutiveFailureCount = 0;

                    continue;
                }

                var pendingLetterLogs = await _appDbContext
                    .MigrationPendingSiteLetterLogs
                    .AsNoTracking()
                    .Where(letterLog => letterLog.LegacyFilePath == legacyFilePath)
                    .ToListAsync();

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(pendingLetterLogs[0].FilePath, download.Content);
                }

                await AddMigratedLetterLogsAsync(pendingLetterLogs, uploadedPath);

                movedCount++;
                letterCount += pendingLetterLogs.Count;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed moving letter file {legacyFilePath}. Error message: {errorMessage}.", legacyFilePath, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} letter files failed in a row. The legacy letters folder or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} letter files with {LetterCount} letters, could not find {UnavailableCount} of them in the legacy letters folder, and failed {FailedCount} files.",
            movedCount, letterCount, unavailableCount, failedCount);
    }

    private async Task AddMigratedLetterLogsAsync(List<PendingSiteLetterLog> pendingLetterLogs, string uploadedPath)
    {
        var letterLogs = new List<SiteLetterLog>();

        foreach (var pendingLetterLog in pendingLetterLogs)
        {
            letterLogs.Add(new SiteLetterLog
            {
                WaterSupplierId = pendingLetterLog.WaterSupplierId,
                SiteId = pendingLetterLog.SiteId,
                ProcessDate = pendingLetterLog.ProcessDate,
                FileName = pendingLetterLog.FileName,
                Page = pendingLetterLog.Page,
                FilePath = uploadedPath
            });
        }

        _appDbContext.SiteLetterLogs.AddRange(letterLogs);

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        finally
        {
            _appDbContext.ChangeTracker.Clear();
        }
    }
}
