using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowComplianceReportService(IBackflowComplianceReportRepository repository) : IBackflowComplianceReportService
{
    public Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        return repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);
    }

    public Task<BackflowComplianceHistoryDto> GetComplianceHistoryAsync(CancellationToken cancellationToken)
    {
        return repository.GetComplianceHistoryAsync(cancellationToken);
    }
}
