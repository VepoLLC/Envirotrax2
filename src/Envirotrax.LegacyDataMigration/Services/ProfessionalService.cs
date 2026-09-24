using Envirotrax.LegacyDataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class ProfessionalService
{
    private const string ScriptsFolder = "Scripts/Professionals";

    private readonly ILogger<ProfessionalService> _logger;
    private readonly AppDbContext _appDbContext;

    public ProfessionalService(
        ILogger<ProfessionalService> logger,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of professionals ---------");

        await ExecuteSqlScriptsAsync();

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
}
