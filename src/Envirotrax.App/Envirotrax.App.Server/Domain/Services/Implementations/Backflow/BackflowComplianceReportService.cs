using System.Globalization;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowComplianceReportService(
    IBackflowComplianceReportRepository repository,
    IBackflowComplianceSnapshotRepository snapshotRepository,
    IPdfTemplateService pdfTemplateService) : IBackflowComplianceReportService
{
    public Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        return repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);
    }

    public async Task<byte[]> GeneratePdfAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        var report = await repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);
        var model = new BackflowComplianceReportExportDto { Report = report, IgnoreLast30Days = ignoreLast30Days };

        return await pdfTemplateService.GenerateAsync("Backflow.BackflowComplianceReport", model);
    }

    public async Task<byte[]> GenerateExcelAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        var report = await repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);

        return BackflowComplianceReportExcelBuilder.Build(report, ignoreLast30Days);
    }

    public async Task<byte[]> GenerateWordAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        var report = await repository.GetComplianceReportAsync(ignoreLast30Days, cancellationToken);

        return BackflowComplianceReportWordBuilder.Build(report, ignoreLast30Days);
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

    public async Task<byte[]> GenerateHistoryPdfAsync(CancellationToken cancellationToken)
    {
        var history = await GetComplianceHistoryAsync(cancellationToken);

        return await pdfTemplateService.GenerateAsync("Backflow.BackflowComplianceHistory", history);
    }

    public async Task<byte[]> GenerateHistoryExcelAsync(CancellationToken cancellationToken)
    {

        var history = await GetComplianceHistoryAsync(cancellationToken);

        return BackflowComplianceHistoryExcelBuilder.Build(history);
    }

    public async Task<byte[]> GenerateHistoryWordAsync(CancellationToken cancellationToken)
    {
        var history = await GetComplianceHistoryAsync(cancellationToken);

        return BackflowComplianceHistoryWordBuilder.Build(history);
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
