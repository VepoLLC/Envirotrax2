using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTripTicketReportRepository
{
    Task<FogSystemReportDto> GetTripTicketReportAsync(FogTripTicketReportDateType dateType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<DateTime?> GetEarliestTripTicketDateAsync(CancellationToken cancellationToken);
}
