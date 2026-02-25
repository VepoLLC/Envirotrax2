
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthService(IHttpContextAccessor contextAccessor)
        : base(contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public IEnumerable<FeatureType> GetAllMyFeatures()
    {
        var claim = _contextAccessor.HttpContext?.User.FindFirstValue("fts");

        if (!string.IsNullOrWhiteSpace(claim))
        {
            return claim
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => (FeatureType)int.Parse(f));
        }

        return [];
    }

    public bool HasAnyFeatures(params FeatureType[] featuresToCheck)
    {
        var features = GetAllMyFeatures();
        return features.Any(f => featuresToCheck.Contains(f));
    }
}