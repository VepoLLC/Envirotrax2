
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly TenantDbContext _dbContext;

    public RolePermissionRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
    {
        return await _dbContext
            .Permissions
            .OrderBy(r => r.SortOrder.HasValue ? r.SortOrder : (int)r.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<RolePermission>> GetAllAsync(int roleId)
    {
        return await _dbContext
            .RolePermissions
            .Include(p => p.Permission)
            .Where(p => p.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<RolePermission> AddOrUpdateAsync(RolePermission rolePermission)
    {
        var dbRecord = await _dbContext
            .RolePermissions
            .SingleOrDefaultAsync(p => p.RoleId == rolePermission.RoleId && p.PermissionId == rolePermission.PermissionId);

        if (dbRecord == null)
        {
            dbRecord = new()
            {
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId
            };

            _dbContext.RolePermissions.Add(dbRecord);
        }

        dbRecord.CanView = rolePermission.CanView;
        dbRecord.CanCreate = rolePermission.CanCreate;
        dbRecord.CanEdit = rolePermission.CanEdit;
        dbRecord.CanDelete = rolePermission.CanDelete;

        await _dbContext.SaveChangesAsync();

        return dbRecord;
    }

    public async Task<IEnumerable<RolePermission>> BulkUpdateAsync(IEnumerable<RolePermission> rolePermissions)
    {
        foreach (var rolePermission in rolePermissions)
        {
            var dbRecord = await _dbContext
                .RolePermissions
                .SingleOrDefaultAsync(p => p.RoleId == rolePermission.RoleId && p.PermissionId == rolePermission.PermissionId);

            if (dbRecord == null)
            {
                dbRecord = new()
                {
                    RoleId = rolePermission.RoleId,
                    PermissionId = rolePermission.PermissionId
                };

                _dbContext.RolePermissions.Add(dbRecord);
            }

            dbRecord.CanView = rolePermission.CanView;
            dbRecord.CanCreate = rolePermission.CanCreate;
            dbRecord.CanEdit = rolePermission.CanEdit;
            dbRecord.CanDelete = rolePermission.CanDelete;
        }

        await _dbContext.SaveChangesAsync();

        return rolePermissions;
    }

    public async Task<IEnumerable<RolePermission>> GetAllByUserAsync(int supplierId, int userId)
    {
        var rolePermissionQuery = from rolePermission in _dbContext.RolePermissions.IgnoreQueryFilters()

                                  join userRole in _dbContext.UserRoles.IgnoreQueryFilters()
                                  on new { rolePermission.WaterSupplierId, rolePermission.RoleId } equals new { userRole.WaterSupplierId, userRole.RoleId }

                                  where rolePermission.WaterSupplierId == supplierId &&
                                    rolePermission.RoleId == userRole.RoleId &&
                                    userRole.WaterSupplierId == supplierId &&
                                    userRole.UserId == userId

                                  select new RolePermission
                                  {
                                      WaterSupplierId = rolePermission.WaterSupplierId,
                                      RoleId = rolePermission.RoleId,
                                      PermissionId = rolePermission.PermissionId,
                                      CanView = rolePermission.CanView,
                                      CanCreate = rolePermission.CanCreate,
                                      CanEdit = rolePermission.CanEdit,
                                      CanDelete = rolePermission.CanDelete
                                  };

        var rolePermissions = await rolePermissionQuery.AsNoTracking().ToListAsync();

        return rolePermissions
            .GroupBy(r => new { r.WaterSupplierId, r.PermissionId })
            .Select(group => new RolePermission
            {
                WaterSupplierId = group.Key.WaterSupplierId,
                PermissionId = group.Key.PermissionId,
                CanView = group.Any(r => r.CanView),
                CanCreate = group.Any(r => r.CanCreate),
                CanEdit = group.Any(r => r.CanEdit),
                CanDelete = group.Any(r => r.CanDelete)
            }).ToList();
    }
}