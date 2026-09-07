using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IWaterSupplierDashboardService
{
    Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken);
    Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken);
    Task<BackflowSubmissionStatsDto> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken);
    Task<BackflowComplianceSnapshotDto?> GetBackflowComplianceAsync(CancellationToken cancellationToken);
    Task<FogInspectionSubmissionStatsDto> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken);
    Task<FogTripTicketSubmissionStatsDto> GetFogTripTicketSubmissionStatsAsync(CancellationToken cancellationToken);
}
