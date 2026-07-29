
using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class WaterSupplierService
{
    private const string V1ScriptsFolder = "Scripts/WaterSuppliers/V1";
    private const string V2ScriptsFolder = "Scripts/WaterSuppliers/V2";

    private readonly ILogger<WaterSupplierService> _logger;
    private readonly LegacyDbService _legacyDb;
    private readonly AppDbContext _appDbContext;

    public WaterSupplierService(
        ILogger<WaterSupplierService> logger,
        LegacyDbService legacyDb,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _legacyDb = legacyDb;
        _appDbContext = appDbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of water suppliers ---------");

        await ExecuteSqlScriptsAsync();

        _logger.LogInformation("--------- Finished migration of water suppliers ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        await ExecuteV2ScriptsAsync();
        await ExecuteV1ScriptsAsync();
    }

    private async Task ExecuteV2ScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", V2ScriptsFolder);

        var scripts = Directory.GetFiles(V2ScriptsFolder, "*.sql");

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            await _appDbContext.Database.ExecuteSqlRawAsync(sql);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", V2ScriptsFolder);
    }

    private async Task ExecuteV1ScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", V1ScriptsFolder);

        var scripts = Directory.GetFiles(V1ScriptsFolder, "*.sql");

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _legacyDb.ExecuteNonQueryAsync(sql);
            _logger.LogInformation("Imported water suppliers. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", V1ScriptsFolder);
    }
}