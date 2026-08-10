using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestReportService(IBackflowTestReportRepository repository) : IBackflowTestReportService
{
    public Task<BackflowTestReportDto> GetTestReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return repository.GetTestReportAsync(fromDate, toDate, cancellationToken);
    }

    public Task<DateTime?> GetEarliestTestDateAsync(CancellationToken cancellationToken)
    {
        return repository.GetEarliestTestDateAsync(cancellationToken);
    }
}
