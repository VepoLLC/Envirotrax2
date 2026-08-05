
using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class WaterSupplierUserService
{
    private const string ScriptsFolder = "Scripts/WaterSupplierUsers";

    private readonly ILogger<WaterSupplierUserService> _logger;
    private readonly AppDbContext _dbContext;

    public WaterSupplierUserService(ILogger<WaterSupplierUserService> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of water supplier users ---------");

        await ExecuteSqlScriptsAsync();

        _logger.LogInformation("--------- Finished migration of water supplier users ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", ScriptsFolder);

        var scripts = Directory.GetFiles(ScriptsFolder, "*.sql").OrderBy(file => file);

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _dbContext.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Imported water supplier users. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }
}