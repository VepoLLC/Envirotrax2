using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionReportRepository
{
    Task<FogSystemReportDto> GetInspectionReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<DateTime?> GetEarliestInspectionDateAsync(CancellationToken cancellationToken);
}
