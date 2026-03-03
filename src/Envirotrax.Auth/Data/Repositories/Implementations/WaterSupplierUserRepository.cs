
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class WaterSupplierUserRepository : IWaterSupplierUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WaterSupplierUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WaterSupplierUser?> GetAsync(int supplierId, int userId)
    {
        var supplierUser = await _dbContext
            .WaterSupplierUsers
            .SingleOrDefaultAsync(s => s.WaterSupplierId == supplierId && s.UserId == userId);

        if (supplierUser == null)
        {
            var parentId = await _dbContext
                    .WaterSuppliers
                    .Where(t => t.Id == supplierId)
                    .Select(t => t.ParentId)
                    .SingleOrDefaultAsync();

            if (parentId.HasValue)
            {
                supplierUser = await GetAsync(parentId.Value, userId);
            }
        }

        return supplierUser;
    }

    public async Task<IEnumerable<RolePermission>> GetAllPermissionsAsync(int supplierId, int userId)
    {
        var rolePermissionQuery = from rolePermission in _dbContext.RolePermissions.IgnoreQueryFilters()

                                  join userRole in _dbContext.AppUserRoles.IgnoreQueryFilters()
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
                                      CanList = rolePermission.CanList,
                                      CanCreate = rolePermission.CanCreate,
                                      CanEdit = rolePermission.CanEdit,
                                      CanDelete = rolePermission.CanDelete
                                  };

        return await rolePermissionQuery
            .AsNoTracking()
            .ToListAsync();
    }
}