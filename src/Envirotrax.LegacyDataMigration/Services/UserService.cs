
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class UserService
{
    private const string BeforeMigrationScriptPath = "Scripts/Users/01_BeforeMigration.sql";

    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly LegacyDbService _legacyDb;
    private readonly AppDbContext _appDbContext;

    public UserService(
        UserManager<AppUser> userManager,
        ILogger<UserService> logger,
        LegacyDbService legacyDb,
        AppDbContext appDbContext)
    {
        _userManager = userManager;
        _logger = logger;
        _legacyDb = legacyDb;
        _appDbContext = appDbContext;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of users ---------");

        await ExecuteSqlScriptsAsync();
        await HashLegacyPasswordsAsync();

        _logger.LogInformation("--------- Finished migration of users ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        await ExecuteBeforeMigrationScriptAsync();

        var folderName = "Scripts/Users";
        _logger.LogInformation("Executing database scripts from {folderName}.", folderName);

        var legacyDataScripts = Directory
            .GetFiles(folderName, "*.sql")
            .Where(file => Path.GetFullPath(file) != Path.GetFullPath(BeforeMigrationScriptPath));

        foreach (var file in legacyDataScripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRpws = await _legacyDb.ExecuteNonQueryAsync(sql);
            _logger.LogInformation("Imported users. Count: {count}", addedRpws);
        }

        _logger.LogInformation("Completed executing database scripts.");
    }

    private async Task ExecuteBeforeMigrationScriptAsync()
    {
        _logger.LogInformation("Executing V2 schema setup script {file}", BeforeMigrationScriptPath);

        var sql = await File.ReadAllTextAsync(BeforeMigrationScriptPath);
        await _appDbContext.Database.ExecuteSqlRawAsync(sql);
    }

    private async Task HashLegacyPasswordsAsync()
    {
        _logger.LogInformation("Hashing legacy passwords imported from V1.");

        var hashedCount = 0;
        var failedCount = 0;

        var users = await _userManager
            .Users
            .Where(user => user.PasswordExpirationDate != null && !user.IsMigratedLegacyPasswordHashed)
            .ToListAsync();

        _logger.LogInformation("Hashing passwords of {Count} users.", users.Count);

        foreach (var user in users)
        {

            try
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, user.PasswordHash!);
                user.IsMigratedLegacyPasswordHashed = true;
                user.ConcurrencyStamp = Guid.NewGuid().ToString();

                hashedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed hashing legacy password of user: {userName}. Error mesage: {errorMessage}.", user.UserName, ex.Message);
                failedCount++;
            }
        }

        _logger.LogInformation("Hashed {HashedCount} records and failed {FailedCount} records.", hashedCount, failedCount);
        _logger.LogInformation("Executing database updates of password hashes.");

        await _appDbContext.SaveChangesAsync();
    }
}