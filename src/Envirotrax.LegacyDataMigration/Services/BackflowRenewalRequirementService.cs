using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class BackflowRenewalRequirementService
{
    private const string ScriptsFolder = "Scripts/BackflowRenewalRequirements";

    private readonly ILogger<BackflowRenewalRequirementService> _logger;
    private readonly AppDbContext _appDbContext;

    public BackflowRenewalRequirementService(
        ILogger<BackflowRenewalRequirementService> logger,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of backflow renewal requirements ---------");

        await ExecuteSqlScriptsAsync();

        _logger.LogInformation("--------- Finished migration of backflow renewal requirements ---------");
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
            _logger.LogInformation("Imported backflow renewal requirements. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }
}
