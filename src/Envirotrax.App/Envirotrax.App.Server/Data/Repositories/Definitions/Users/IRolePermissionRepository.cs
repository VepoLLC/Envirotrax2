
using Envirotrax.App.Server.Data.Models.Users;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Users;

public interface IRolePermissionRepository
{
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();

    Task<IEnumerable<RolePermission>> GetAllAsync(int roleId);

    Task<RolePermission> AddOrUpdateAsync(RolePermission rolePermission);

    Task<IEnumerable<RolePermission>> BulkUpdateAsync(IEnumerable<RolePermission> rolePermissions);
}