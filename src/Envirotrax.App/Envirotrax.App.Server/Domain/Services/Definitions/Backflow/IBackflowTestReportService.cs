using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTestReportService
{
    Task<BackflowTestReportDto> GetTestReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<DateTime?> GetEarliestTestDateAsync(CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<byte[]> GenerateExcelAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);

    Task<byte[]> GenerateWordAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
}
