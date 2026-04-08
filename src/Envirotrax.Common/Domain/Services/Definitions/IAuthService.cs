
using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IAuthService : ITenantProvidersService
{
    IEnumerable<FeatureType> GetAllMyFeatures();
    IEnumerable<RolePermissionDto> GetAllMyPermissions();
    IEnumerable<string> GetAllMyRoles();

    bool HasAnyFeatures(params FeatureType[] featuresToCheck);
    bool HasAnyPermission(PermissionAction action, params PermissionType[] permissionTypes);
    bool HasAnyRole(params string[] roles);
}