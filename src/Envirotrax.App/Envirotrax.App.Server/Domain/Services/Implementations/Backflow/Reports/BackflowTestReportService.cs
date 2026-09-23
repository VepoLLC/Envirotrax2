using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestReportService(IBackflowTestReportRepository repository, IPdfTemplateService pdfTemplateService) : IBackflowTestReportService
{
    public Task<BackflowTestReportDto> GetTestReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return repository.GetTestReportAsync(fromDate, toDate, cancellationToken);
    }

    public Task<DateTime?> GetEarliestTestDateAsync(CancellationToken cancellationToken)
    {
        return repository.GetEarliestTestDateAsync(cancellationToken);
    }

    public async Task<byte[]> GeneratePdfAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var report = await repository.GetTestReportAsync(fromDate, toDate, cancellationToken);
        var model = new BackflowTestReportExportDto { Report = report, FromDate = fromDate, ToDate = toDate };

        return await pdfTemplateService.GenerateAsync("Backflow.BackflowTestReport", model);
    }

    public async Task<byte[]> GenerateExcelAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var report = await repository.GetTestReportAsync(fromDate, toDate, cancellationToken);

        return BackflowTestReportExcelBuilder.Build(report, fromDate, toDate);
    }

    public async Task<byte[]> GenerateWordAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var report = await repository.GetTestReportAsync(fromDate, toDate, cancellationToken);

        return BackflowTestReportWordBuilder.Build(report, fromDate, toDate);
    }
}
