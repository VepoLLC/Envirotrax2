
using Envirotrax.Common;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class UserAccessDto
{
    public IEnumerable<FeatureType> Features { get; set; } = null!;
}