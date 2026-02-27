
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Users;

public interface IRolePermissionService
{
    Task<IEnumerable<ReferencedPermissionDto>> GetAllPermissionsAsync();

    Task<IEnumerable<RolePermissionDto>> GetAllAsync(int roleId);

    Task<IEnumerable<RolePermissionDto>> GetAllMyPermissionsAsync();

    Task<RolePermissionDto> AddOrUpdateAsync(RolePermissionDto rolePermission);
}