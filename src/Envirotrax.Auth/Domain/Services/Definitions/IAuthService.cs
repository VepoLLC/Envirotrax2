
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.Auth.Areas.OpenIdConnect.Services.Definitions;

public interface IAuthService : ITenantProvidersService
{
    Task<UserAccessDto> GetAccessSettingsAsync(int userId, int? waterSupplierId, int? contractorId);
}