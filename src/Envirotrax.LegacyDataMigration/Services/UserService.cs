
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class UserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly LegacyDbService _legacyDb;

    public UserService(
        UserManager<AppUser> userManager,
        ILogger<UserService> logger,
        LegacyDbService legacyDb)
    {
        _userManager = userManager;
        _logger = logger;
        _legacyDb = legacyDb;
    }

    public async Task MigrateAsync()
    {
        await ExecuteSqlScriptsAsync();
        await HashLegacyPasswordsAsync();
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        var folderName = "Scripts/Users";
        _logger.LogInformation("Executing database scripts from {folderName}.", folderName);

        foreach (var file in Directory.GetFiles(folderName, "**.sql"))
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRpws = await _legacyDb.ExecuteNonQueryAsync(sql);
            _logger.LogInformation("Imported users. Count: {count}", addedRpws);
        }

        _logger.LogInformation("Completed executing database scripts.");
    }

    private async Task HashLegacyPasswordsAsync()
    {
        _logger.LogInformation("Hashing legacy passwords imported from V1.");

        var users = await _userManager
            .Users
            .Where(user => !user.IsMigratedLegacyPasswordHashed)
            .ToListAsync();

        foreach (var user in users)
        {
            try
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, user.PasswordHash!);
                user.IsMigratedLegacyPasswordHashed = true;

                await _userManager.UpdateAsync(user);
            }
            catch (Exception)
            {
                _logger.LogWarning("Failed hashing legacy password of user: {userName}", user.UserName);
            }
        }
    }
}