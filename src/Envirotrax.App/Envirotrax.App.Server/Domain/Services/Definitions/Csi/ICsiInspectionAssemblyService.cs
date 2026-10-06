using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionAssemblyService
{
    Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<List<CsiInspectionAssemblyDto>> InitializeAsync(int siteId, string submissionId, CancellationToken cancellationToken);
    Task<CsiInspectionAssemblyDto> AddAsync(CsiInspectionAssemblyRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, string submissionId, CancellationToken cancellationToken);
    Task UpdateVisuallyIdentifiedAsync(CsiInspectionVisuallyIdentifiedRequest request, CancellationToken cancellationToken);
    Task LinkToInspectionAsync(CsiInspection inspection, CancellationToken cancellationToken);
    Task MarkPaidAsync(IEnumerable<CsiInspection> inspections, string transactionId, DateTime transactionDate, CancellationToken cancellationToken);
    Task DeleteByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
}
