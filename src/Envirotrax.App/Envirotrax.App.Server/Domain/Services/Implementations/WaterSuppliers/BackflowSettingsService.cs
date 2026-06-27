using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class BackflowSettingsService : Service<BackflowSettings, BackflowSettingsDto>, IBackflowSettingsService
{
    private readonly IBackflowSettingsRepository _repository;

    public BackflowSettingsService(IMapper mapper, IBackflowSettingsRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public async Task<BackflowTestingSettingsDto> GetTestingSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetTestingSettingsAsync(waterSupplierId, cancellationToken);

        return settings ?? new BackflowTestingSettingsDto();
    }
}
