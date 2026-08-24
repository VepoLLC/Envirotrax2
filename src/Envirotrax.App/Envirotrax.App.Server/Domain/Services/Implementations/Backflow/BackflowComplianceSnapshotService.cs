using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// Computes and persists Backflow compliance snapshots. Tenant handling is ambient: every read/write
// runs through the current tenant context, so the caller (later, the TaskRunner endpoint) is
// responsible for setting the target supplier before invoking these methods.
public class BackflowComplianceSnapshotService(
    IBackflowComplianceReportRepository reportRepository,
    IBackflowComplianceSnapshotRepository snapshotRepository) : IBackflowComplianceSnapshotService
{
    // Compute and persist a single month's snapshot. The month is supplied by the caller (e.g. the
    // monthly job's target month) and normalized to the first of that month, so only the requested
    // month is computed rather than the full history.
    public async Task GenerateSnapshotAsync(DateTime reportDate, CancellationToken cancellationToken)
    {
        var month = FirstOfMonth(reportDate);

        var points = await reportRepository.ComputeHistoryPointsAsync([month], cancellationToken);

        var snapshot = MapToSnapshot(points.Single());

        await snapshotRepository.UpsertAsync(snapshot, cancellationToken);
    }

    // One-time seed of the full compliance history. Reconstructs every month from the earliest test
    // through the current month (no window cap), so the snapshot table holds all history like V1's
    // WaterSupplierReports (the 48-month limit is a chart-only concern on the client). Written in a
    // single bulk upsert; idempotent, so it can be re-run safely.
    public async Task BackfillAsync(CancellationToken cancellationToken)
    {
        var history = await reportRepository.ReconstructComplianceHistoryAsync(cancellationToken);

        var snapshots = history.Points
            .Select(MapToSnapshot)
            .ToList();

        await snapshotRepository.BulkUpsertAsync(snapshots, cancellationToken);
    }

    private static BackflowComplianceSnapshot MapToSnapshot(BackflowComplianceHistoryPointDto point)
    {
        return new BackflowComplianceSnapshot
        {
            ReportDate = new DateTime(point.Year, point.Month, 1),
            Total = point.Total,
            Compliant = point.Compliant
        };
    }

    private static DateTime FirstOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }
}
