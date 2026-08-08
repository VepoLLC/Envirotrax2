using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogSystemReportService : IFogSystemReportService
{
    private readonly IFogTripTicketReportRepository _tripTicketReportRepository;
    private readonly IFogInspectionReportRepository _inspectionReportRepository;

    public FogSystemReportService( IFogTripTicketReportRepository tripTicketReportRepository,IFogInspectionReportRepository inspectionReportRepository)
    {
        _tripTicketReportRepository = tripTicketReportRepository;
        _inspectionReportRepository = inspectionReportRepository;
    }

    public Task<FogSystemReportDto> GetTripTicketReportAsync(FogTripTicketReportDateType dateType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return _tripTicketReportRepository.GetTripTicketReportAsync(dateType, fromDate, toDate, cancellationToken);
    }

    public Task<DateTime?> GetEarliestTripTicketDateAsync(CancellationToken cancellationToken)
    {
        return _tripTicketReportRepository.GetEarliestTripTicketDateAsync(cancellationToken);
    }

    public Task<FogSystemReportDto> GetInspectionReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return _inspectionReportRepository.GetInspectionReportAsync(fromDate, toDate, cancellationToken);
    }

    public Task<DateTime?> GetEarliestInspectionDateAsync(CancellationToken cancellationToken)
    {
        return _inspectionReportRepository.GetEarliestInspectionDateAsync(cancellationToken);
    }
}
