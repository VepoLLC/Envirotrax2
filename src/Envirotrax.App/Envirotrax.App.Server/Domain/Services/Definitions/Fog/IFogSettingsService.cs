using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogSettingsService
{
    Task<ProfessionalFogSettingsDto> GetSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);
}
