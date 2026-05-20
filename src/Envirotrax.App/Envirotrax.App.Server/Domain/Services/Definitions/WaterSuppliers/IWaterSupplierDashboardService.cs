using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IWaterSupplierDashboardService
{
    Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken);
}
