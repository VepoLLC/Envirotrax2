
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.Auth.Domain.Services.Definitions;

public interface IAuthService : ITenantProvidersService
{
    Task SetSecurityPropertiesAsync(ClaimsPrincipal principal, int userId, int? waterSupplierId, int? professionalId);
}