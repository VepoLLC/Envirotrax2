
using Envirotrax.Auth.Areas.OpenIdConnect.Services.Definitions;
using Envirotrax.Common.Data.Services.Implementations;

namespace Envirotrax.Auth.Areas.OpenIdConnect.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    public AuthService(IHttpContextAccessor contextAccessor)
        : base(contextAccessor)
    {
    }
}