using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog;

public interface IFogSystemReportService
{
    Task<FogSystemReportDto> GetTripTicketReportAsync(FogTripTicketReportDateType dateType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<DateTime?> GetEarliestTripTicketDateAsync(CancellationToken cancellationToken);

    Task<FogSystemReportDto> GetInspectionReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<DateTime?> GetEarliestInspectionDateAsync(CancellationToken cancellationToken);
}
