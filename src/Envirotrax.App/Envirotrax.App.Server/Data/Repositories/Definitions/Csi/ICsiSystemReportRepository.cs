using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiSystemReportRepository
{
    Task<CsiSystemReportDto> GetSystemReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
}
