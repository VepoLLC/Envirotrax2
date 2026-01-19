
using Envirotrax.Common.Data.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    public AuthService(IHttpContextAccessor contextAccessor)
        : base(contextAccessor)
    {
    }
}