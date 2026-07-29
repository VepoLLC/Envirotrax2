
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class UserService
{
    private const string V1ScriptsFolder = "Scripts/Users/V1";
    private const string V2ScriptsFolder = "Scripts/Users/V2";

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
            _logger.LogInformation("Imported users. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", V1ScriptsFolder);
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