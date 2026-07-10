using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IWaterSupplierDashboardRepository
{
    Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken);
    Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken);
    Task<BackflowSubmissionStatsDto> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken);
    Task<FogInspectionSubmissionStatsDto> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken);
}
