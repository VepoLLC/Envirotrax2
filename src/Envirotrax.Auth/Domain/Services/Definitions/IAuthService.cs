
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.Auth.Areas.OpenIdConnect.Services.Definitions;

public interface IAuthService : ITenantProvidersService
{
    Task SetSecurityPropertiesAsync(ClaimsPrincipal principal, int userId, int? waterSupplierId, int? contractorId);
}