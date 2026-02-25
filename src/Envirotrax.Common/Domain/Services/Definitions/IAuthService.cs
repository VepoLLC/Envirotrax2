
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IAuthService : ITenantProvidersService
{
    IEnumerable<FeatureType> GetAllMyFeatures();
    bool HasAnyFeatures(params FeatureType[] featuresToCheck);
}