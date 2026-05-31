using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiSystemReportService
{
    Task<CsiSystemReportDto> GetSystemReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
}
