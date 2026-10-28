using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class CsiSettingsService : Service<CsiSettings, CsiSettingsDto>, ICsiSettingsService
{
    private readonly ICsiSettingsRepository _repository;

    public CsiSettingsService(IMapper mapper, ICsiSettingsRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<CsiSettingsDto> AddOrUpdateAsync(int waterSupplierId, CsiSettingsDto settings)
    {
        var model = MapToModel(settings)!;
        var saved = await _repository.AddOrUpdateAsync(waterSupplierId, model);

        return MapToDto(saved)!;
    }
}
