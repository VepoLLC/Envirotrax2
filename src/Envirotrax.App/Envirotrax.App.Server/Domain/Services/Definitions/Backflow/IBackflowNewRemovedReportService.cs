using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowNewRemovedReportService
{
    Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(CancellationToken cancellationToken);

    Task<byte[]> GenerateExcelAsync(CancellationToken cancellationToken);

    Task<byte[]> GenerateWordAsync(CancellationToken cancellationToken);
}
