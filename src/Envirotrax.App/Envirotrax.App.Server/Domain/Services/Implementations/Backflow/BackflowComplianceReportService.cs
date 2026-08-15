using System.Globalization;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowComplianceReportService(
    IBackflowComplianceReportRepository repository,
    IBackflowComplianceSnapshotRepository snapshotRepository) : IBackflowComplianceReportService
{
    public Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        return repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);
    }

    // Reads the persisted monthly snapshots (written by the TaskRunner job) instead of reconstructing
    // the history from every test on each request. Rows come back ordered by month; NonCompliant and
    // Percentage are derived here, mirroring how the reconstruct path built each point.
    public async Task<BackflowComplianceHistoryDto> GetComplianceHistoryAsync(CancellationToken cancellationToken)
    {
        var snapshots = await snapshotRepository.GetAllAsync(cancellationToken);

        var result = new BackflowComplianceHistoryDto();

        foreach (var snapshot in snapshots)
        {
            result.Points.Add(BuildPoint(snapshot));
        }

        return result;
    }

    private static BackflowComplianceHistoryPointDto BuildPoint(BackflowComplianceSnapshot snapshot)
    {
        var nonCompliant = snapshot.Total - snapshot.Compliant;
        var percentage = snapshot.Total > 0 ? Math.Round((double)snapshot.Compliant / snapshot.Total * 100) : 0;

        return new BackflowComplianceHistoryPointDto
        {
            Year = snapshot.ReportDate.Year,
            Month = snapshot.ReportDate.Month,
            Label = snapshot.ReportDate.ToString("MMM yyyy", CultureInfo.InvariantCulture),
            Total = snapshot.Total,
            Compliant = snapshot.Compliant,
            NonCompliant = nonCompliant,
            Percentage = percentage
        };
    }
}
