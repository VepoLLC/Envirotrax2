using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogSettingsRepository
{
    Task<ProfessionalFogSettingsDto?> GetSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);
}
