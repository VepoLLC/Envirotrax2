using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogSettingsService : IFogSettingsService
{
    private readonly IFogSettingsRepository _repository;

    public FogSettingsService(IFogSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProfessionalFogSettingsDto> GetSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetSettingsAsync(waterSupplierId, cancellationToken);

        return settings ?? new ProfessionalFogSettingsDto();
    }
}
