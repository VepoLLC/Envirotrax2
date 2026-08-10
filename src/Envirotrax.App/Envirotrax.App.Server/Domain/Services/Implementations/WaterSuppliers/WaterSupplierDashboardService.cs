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

    public Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetStatsAsync(cancellationToken);

        return stats;
    }

    public Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetCsiSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<BackflowSubmissionStatsDto> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetBackflowSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<FogInspectionSubmissionStatsDto> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetFogInspectionSubmissionStatsAsync(cancellationToken);

        return stats;
    }

    public Task<FogTripTicketSubmissionStatsDto> GetFogTripTicketSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var stats = _repository.GetFogTripTicketSubmissionStatsAsync(cancellationToken);

        return stats;
    }
}
