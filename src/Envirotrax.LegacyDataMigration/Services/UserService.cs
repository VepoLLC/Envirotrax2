
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class UserService
{
    private const string ScriptsFolder = "Scripts/Users";

    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly AppIdentityDbContext _identityDbContext;

    public UserService(
        UserManager<AppUser> userManager,
        ILogger<UserService> logger,
        AppIdentityDbContext identityDbContext)
    {
        _userManager = userManager;
        _logger = logger;
        _identityDbContext = identityDbContext;
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
        _logger.LogInformation("Executing database scripts from {folderName}.", ScriptsFolder);

        var scripts = Directory.GetFiles(ScriptsFolder, "*.sql").OrderBy(file => file);

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _identityDbContext.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Imported users. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
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

        await _identityDbContext.SaveChangesAsync();
    }
}