using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionAssemblyService
{
    Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<List<CsiInspectionAssemblyDto>> GetForFormAsync(int siteId, int? inspectionId, CancellationToken cancellationToken);
    Task SaveForInspectionAsync(CsiInspection inspection, CreateCsiInspectionDto request, CancellationToken cancellationToken);
    Task MarkPaidAsync(IEnumerable<CsiInspection> inspections, string transactionId, DateTime transactionDate, CancellationToken cancellationToken);
    Task DeleteByInspectionAsync(int inspectionId, int professionalId, CancellationToken cancellationToken);
}
