using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class CompanyService
{
    private const string ScriptsFolder = "Scripts/Companies";

    private readonly ILogger<CompanyService> _logger;
    private readonly AppDbContext _appDbContext;

    public CompanyService(
        ILogger<CompanyService> logger,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of CRM companies ---------");

        await ExecuteSqlScriptsAsync();

        _logger.LogInformation("--------- Finished migration of CRM companies ---------");
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
            _logger.LogInformation("Imported CRM companies and their additional contacts. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }
}
