using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiSystemReportService(ICsiSystemReportRepository repository) : ICsiSystemReportService
{
    public Task<CsiSystemReportDto> GetSystemReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var result = repository.GetSystemReportAsync(fromDate, toDate, cancellationToken);

        return result;
    }
}
