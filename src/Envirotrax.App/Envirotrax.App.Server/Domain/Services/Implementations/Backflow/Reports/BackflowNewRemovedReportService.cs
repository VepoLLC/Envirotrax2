using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowNewRemovedReportService(
    IBackflowNewRemovedReportRepository repository,
    IPdfTemplateService pdfTemplateService) : IBackflowNewRemovedReportService
{
    public Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        return repository.GetNewRemovedAsync(cancellationToken);
    }

    public async Task<byte[]> GeneratePdfAsync(CancellationToken cancellationToken)
    {
        var report = await repository.GetNewRemovedAsync(cancellationToken);

        return await pdfTemplateService.GenerateAsync("Backflow.BackflowNewRemoved", report);
    }

    public async Task<byte[]> GenerateExcelAsync(CancellationToken cancellationToken)
    {
        var report = await repository.GetNewRemovedAsync(cancellationToken);

        return BackflowNewRemovedExcelBuilder.Build(report);
    }

    public async Task<byte[]> GenerateWordAsync(CancellationToken cancellationToken)
    {
        var report = await repository.GetNewRemovedAsync(cancellationToken);

        return BackflowNewRemovedWordBuilder.Build(report);
    }
}
