
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class UserRepository : Repository<WaterSupplierUser>, IUserRepository
{
    public UserRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<AdminWaterSupplierUserAccountDto>> GetAccountsWithPermissionsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var accounts = await LoadAccountsAsync(waterSupplierId, cancellationToken);
        var permissionsByUser = await LoadPermissionsByUserAsync(waterSupplierId, cancellationToken);

        foreach (var account in accounts)
        {
            if (permissionsByUser.TryGetValue(account.Id, out var permissions))
            {
                account.Permissions = permissions;
            }
        }

        return accounts;
    }

    private async Task<List<AdminWaterSupplierUserAccountDto>> LoadAccountsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await Entity
            .AsNoTracking()
            .Where(user => user.WaterSupplierId == waterSupplierId)
            .OrderBy(user => user.ContactName)
            .Select(user => new AdminWaterSupplierUserAccountDto
            {
                Id = user.UserId,
                ContactName = user.ContactName,
                EmailAddress = user.EmailAddress,
                CellNumber = user.CellNumber
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<Dictionary<int, List<UserAccountPermissionDto>>> LoadPermissionsByUserAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var permissionRows = await (
            from userRole in DbContext.UserRoles.AsNoTracking()
            where userRole.WaterSupplierId == waterSupplierId
            join rolePermission in DbContext.RolePermissions.AsNoTracking().Where(role => role.WaterSupplierId == waterSupplierId)
                on userRole.RoleId equals rolePermission.RoleId
            select new
            {
                userRole.UserId,
                rolePermission.PermissionId,
                rolePermission.CanView,
                rolePermission.CanModify
            })
            .ToListAsync(cancellationToken);

        return permissionRows
            .GroupBy(row => row.UserId)
            .ToDictionary(
                userGroup => userGroup.Key,
                userGroup => userGroup
                    .GroupBy(row => row.PermissionId)
                    .Select(permissionGroup => new UserAccountPermissionDto
                    {
                        Permission = permissionGroup.Key,
                        CanView = permissionGroup.Any(row => row.CanView),
                        CanModify = permissionGroup.Any(row => row.CanModify)
                    })
                    .ToList());
    }
}
