using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierDashboardService : IWaterSupplierDashboardService
{
    private readonly IWaterSupplierDashboardRepository _repository;

    public WaterSupplierDashboardService(IWaterSupplierDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        return await _repository.GetStatsAsync(cancellationToken);
    }
}
