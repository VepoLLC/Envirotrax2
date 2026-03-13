
using Envirotrax.Common;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class UserAccessDto
{
    public IEnumerable<FeatureType> Features { get; set; } = [];
    public IEnumerable<Common.Domain.DataTransferObjects.RolePermissionDto> Permissions { get; set; } = [];
}