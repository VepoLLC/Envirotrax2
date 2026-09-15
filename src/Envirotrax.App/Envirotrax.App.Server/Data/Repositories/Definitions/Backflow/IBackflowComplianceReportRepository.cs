using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowComplianceReportRepository
{
    Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken);

    Task<BackflowComplianceHistoryDto> ReconstructComplianceHistoryAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<BackflowComplianceHistoryPointDto>> ComputeHistoryPointsAsync(IReadOnlyCollection<DateTime> months, CancellationToken cancellationToken);
}
