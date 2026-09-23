using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowComplianceSnapshotService(
    IBackflowComplianceReportRepository reportRepository,
    IBackflowComplianceSnapshotRepository snapshotRepository) : IBackflowComplianceSnapshotService
{
    public async Task GenerateSnapshotAsync(DateTime reportDate, CancellationToken cancellationToken)
    {
        var counts = await reportRepository.CountComplianceAsync(reportDate, cancellationToken);

        var snapshot = new BackflowComplianceSnapshot
        {
            ReportDate = reportDate,
            Total = counts.Total,
            Compliant = counts.Compliant
        };

        await snapshotRepository.UpsertAsync(snapshot, cancellationToken);
    }
}
