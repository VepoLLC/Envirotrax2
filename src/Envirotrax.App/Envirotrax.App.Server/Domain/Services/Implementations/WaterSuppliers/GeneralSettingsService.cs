
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class GeneralSettingsService : Service<GeneralSettings, GeneralSettingsDto>, IGeneralSettingsService
{
    private readonly IGeneralSettingsRepository _repository;

    public GeneralSettingsService(IMapper mapper, IGeneralSettingsRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<GeneralSettingsDto> AddOrUpdateAsync(int waterSupplierId, GeneralSettingsDto settings)
    {
        var model = MapToModel(settings)!;
        var saved = await _repository.AddOrUpdateAsync(waterSupplierId, model);

        return MapToDto(saved)!;
    }
}
