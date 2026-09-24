using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowComplianceReportService
{
    Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken);

    Task<BackflowComplianceHistoryDto> GetComplianceHistoryAsync(CancellationToken cancellationToken);

    Task<BackflowComplianceSnapshotDto?> GetLatestComplianceAsync(CancellationToken cancellationToken);

    Task<byte[]> GeneratePdfAsync(bool ignoreLast30Days, CancellationToken cancellationToken);

    Task<byte[]> GenerateExcelAsync(bool ignoreLast30Days, CancellationToken cancellationToken);

    Task<byte[]> GenerateWordAsync(bool ignoreLast30Days, CancellationToken cancellationToken);

    Task<byte[]> GenerateHistoryPdfAsync(CancellationToken cancellationToken);

    Task<byte[]> GenerateHistoryExcelAsync(CancellationToken cancellationToken);

    Task<byte[]> GenerateHistoryWordAsync(CancellationToken cancellationToken);
}
