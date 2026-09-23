using Envirotrax.Common;
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class WaterSupplierUserService
{
    private const string ScriptsFolder = "Scripts/WaterSupplierUsers";

    private const int LegacyPermissionDeny = 0;
    private const int LegacyPermissionModify = 2;

    // Maps each legacy PermissionsXxx column to its new PermissionType. Columns with no equivalent
    // PermissionType (WiseGuys evaluations/reports, FOG licenses) are intentionally left out.
    private static readonly (Func<LegacyPermissions, int> GetValue, PermissionType PermissionType)[] PermissionColumns =
    [
        (permissions => permissions.PermissionsAccountInformation, PermissionType.AccountInformation),
        (permissions => permissions.PermissionsUserAccounts, PermissionType.Users),
        (permissions => permissions.PermissionsSettings, PermissionType.Settings),
        (permissions => permissions.PermissionsNotifications, PermissionType.Notifications),
        (permissions => permissions.PermissionsSites, PermissionType.Sites),
        (permissions => permissions.PermissionsLicenses, PermissionType.Licenses),
        (permissions => permissions.PermissionsCsiInspections, PermissionType.CsiInspections),
        (permissions => permissions.PermissionsCsiInspectorManagement, PermissionType.CsiInspectors),
        (permissions => permissions.PermissionsCsiReports, PermissionType.CsiReports),
        (permissions => permissions.PermissionsBackflowTests, PermissionType.BackflowTests),
        (permissions => permissions.PermissionsBackflowBpatManagement, PermissionType.BackflowTesters),
        (permissions => permissions.PermissionsBackflowOutOfService, PermissionType.BackflowOutOfService),
        (permissions => permissions.PermissionsBackflowReports, PermissionType.BackflowReports),
        (permissions => permissions.PermissionsFogTickets, PermissionType.FogTripTickets),
        (permissions => permissions.PermissionsFogVehicleManagement, PermissionType.FogVehicles),
        (permissions => permissions.PermissionsFogTransporterManagement, PermissionType.FogTransporters),
        (permissions => permissions.PermissionsFogInspections, PermissionType.FogInspections),
        (permissions => permissions.PermissionsFogInspectorManagement, PermissionType.FogInspectors),
        (permissions => permissions.PermissionsFogReports, PermissionType.FogReports),
    ];

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
        await MigratePermissionsAsync();

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

    // For each water supplier, groups users by their exact legacy permission combination (looked up from
    // Vepo.dbo.WaterSupplierUserAccounts via LegacyRecordId) and creates one Role with matching RolePermissions
    // per distinct combination, then assigns users to it.
    private async Task MigratePermissionsAsync()
    {
        _logger.LogInformation("Migrating legacy user permissions into roles.");

        var assignedUserIds = await _dbContext.UserRoles.Select(userRole => userRole.UserId).ToListAsync();
        var unassignedUsers = await _dbContext.WaterSupplierUsers
            .Where(user => user.LegacyRecordId != null && !assignedUserIds.Contains(user.UserId))
            .ToListAsync();

        var legacyPermissionsByRecordId = await GetLegacyPermissionsByRecordIdAsync();

        var createdRoleCount = 0;
        var assignedUserCount = 0;
        var skippedUserCount = 0;

        foreach (var supplierUsers in unassignedUsers.GroupBy(user => user.WaterSupplierId))
        {
            var roleNumber = await _dbContext.Roles.CountAsync(role => role.WaterSupplierId == supplierUsers.Key);

            var usersWithPermissions = new List<(WaterSupplierUser User, LegacyPermissions Permissions)>();

            foreach (var user in supplierUsers)
            {
                if (legacyPermissionsByRecordId.TryGetValue(user.LegacyRecordId!.Value, out var permissions))
                {
                    usersWithPermissions.Add((user, permissions));
                }
                else
                {
                    _logger.LogWarning("Skipping permission migration for user {UserId}: no matching legacy record {LegacyRecordId}.", user.UserId, user.LegacyRecordId);
                    skippedUserCount++;
                }
            }

            foreach (var group in usersWithPermissions.GroupBy(pair => GetPermissionSignature(pair.Permissions)))
            {
                roleNumber++;

                var role = new Role
                {
                    WaterSupplierId = supplierUsers.Key,
                    Name = $"Migrated Role {roleNumber}"
                };

                _dbContext.Roles.Add(role);
                AddRolePermissions(role, group.First().Permissions);

                foreach (var (user, _) in group)
                {
                    _dbContext.UserRoles.Add(new UserRole
                    {
                        WaterSupplierId = supplierUsers.Key,
                        UserId = user.UserId,
                        Role = role
                    });

                    assignedUserCount++;
                }

                createdRoleCount++;
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Created {RoleCount} roles, assigned {UserCount} users, and skipped {SkippedCount} users with no matching legacy record.",
            createdRoleCount, assignedUserCount, skippedUserCount);
    }

    private async Task<Dictionary<int, LegacyPermissions>> GetLegacyPermissionsByRecordIdAsync()
    {
        var rows = await _dbContext.Database.SqlQuery<LegacyPermissions>($"""
            SELECT
                ID as Id,
                PermissionsAccountInformation,
                PermissionsUserAccounts,
                PermissionsSettings,
                PermissionsNotifications,
                PermissionsSites,
                PermissionsLicenses,
                PermissionsCsiInspections,
                PermissionsCsiInspectorManagement,
                PermissionsCsiReports,
                PermissionsBackflowTests,
                PermissionsBackflowBpatManagement,
                PermissionsBackflowOutOfService,
                PermissionsBackflowReports,
                PermissionsFogTickets,
                PermissionsFogVehicleManagement,
                PermissionsFogTransporterManagement,
                PermissionsFogInspections,
                PermissionsFogInspectorManagement,
                PermissionsFogReports
            FROM Vepo.dbo.WaterSupplierUserAccounts
            """)
            .ToListAsync();

        return rows.ToDictionary(row => row.Id);
    }

    private void AddRolePermissions(Role role, LegacyPermissions permissions)
    {
        foreach (var (getValue, permissionType) in PermissionColumns)
        {
            var level = getValue(permissions);

            if (level == LegacyPermissionDeny)
            {
                continue;
            }

            _dbContext.RolePermissions.Add(new RolePermission
            {
                WaterSupplierId = role.WaterSupplierId,
                Role = role,
                PermissionId = permissionType,
                CanView = true,
                CanModify = level == LegacyPermissionModify,
                CanDelete = false
            });
        }
    }

    private static string GetPermissionSignature(LegacyPermissions permissions)
    {
        return string.Join(",", PermissionColumns.Select(column => column.GetValue(permissions)));
    }

    private sealed record LegacyPermissions(
        int Id,
        int PermissionsAccountInformation,
        int PermissionsUserAccounts,
        int PermissionsSettings,
        int PermissionsNotifications,
        int PermissionsSites,
        int PermissionsLicenses,
        int PermissionsCsiInspections,
        int PermissionsCsiInspectorManagement,
        int PermissionsCsiReports,
        int PermissionsBackflowTests,
        int PermissionsBackflowBpatManagement,
        int PermissionsBackflowOutOfService,
        int PermissionsBackflowReports,
        int PermissionsFogTickets,
        int PermissionsFogVehicleManagement,
        int PermissionsFogTransporterManagement,
        int PermissionsFogInspections,
        int PermissionsFogInspectorManagement,
        int PermissionsFogReports);
}
