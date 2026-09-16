using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class ProfessionalService
{
    private const string ScriptsFolder = "Scripts/Professionals";

    // The legacy file server or Azure Storage being down fails every row the same way. Stopping early
    // keeps a dead host from costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    private readonly ILogger<ProfessionalService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly LegacyFileServerService _legacyFileServerService;
    private readonly BlobStorageService _blobStorageService;

    public ProfessionalService(
        ILogger<ProfessionalService> logger,
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
        _logger.LogInformation("--------- Starting migration of professionals ---------");

        await ExecuteSqlScriptsAsync();
        await MoveInsuranceFilesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of professionals ---------");
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
            _logger.LogInformation("Imported professional records. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

  
    private async Task MoveInsuranceFilesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy insurance policy files into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no insurance policy file was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        var pendingInsurances = await _appDbContext
            .ProfessionalInsurances
            .AsNoTracking()
            .Where(insurance => insurance.LegacyFilePath != null
                && insurance.FilePath == ""
                && insurance.LegacyRecordId != null)
            .OrderBy(insurance => insurance.Id)
            .ToListAsync();

        _logger.LogInformation("Moving files of {Count} insurance policies.", pendingInsurances.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;

        foreach (var insurance in pendingInsurances)
        {
            try
            {
                var download = await _legacyFileServerService.DownloadAsync(insurance.LegacyFilePath!);

                if (download.Content == null)
                {
                    _logger.LogWarning(
                        "File of insurance policy {insuranceId} was not moved because {reason}. Legacy path: {legacyFilePath}.",
                        insurance.Id, download.UnavailableReason, insurance.LegacyFilePath);

                    unavailableCount++;

                    // The server answered, so it is alive. Only failures that look like an outage count
                    // towards stopping the run early.
                    consecutiveFailureCount = 0;

                    continue;
                }

                // Same folders as the V2 ProfessionalInsuranceService, with the V1 record id in place of
                // a new GUID, so a retry overwrites the same blob instead of orphaning one.
                var blobPath = $"professionals/{insurance.ProfessionalId}/insurances/legacy-{insurance.LegacyRecordId}{Path.GetExtension(insurance.LegacyFilePath!)}";

                string uploadedPath;

                using (download.Content)
                {
                    uploadedPath = await _blobStorageService.UploadAsync(blobPath, download.Content);
                }

                // Written one row at a time, after the upload, so a run that dies half way keeps every
                // file it already moved and never points a link at a blob that is not there.
                await _appDbContext
                    .ProfessionalInsurances
                    .Where(pendingInsurance => pendingInsurance.Id == insurance.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(pendingInsurance => pendingInsurance.FilePath, uploadedPath));

                movedCount++;
                consecutiveFailureCount = 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed moving file of insurance policy: {insuranceId}. Error message: {errorMessage}.", insurance.Id, ex.Message);

                failedCount++;
                consecutiveFailureCount++;

                if (consecutiveFailureCount >= MaxConsecutiveFailures)
                {
                    _logger.LogError(
                        "Stopping after {failureCount} insurance policy files failed in a row. The legacy file server or Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                        consecutiveFailureCount);

                    break;
                }
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} insurance policy files, could not get {UnavailableCount} off the legacy file server, and failed {FailedCount} files.",
            movedCount, unavailableCount, failedCount);
    }
}
