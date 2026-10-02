using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionAssemblyService
{
    Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<List<CsiInspectionAssemblyDto>> GetForSiteAsync(int siteId, CancellationToken cancellationToken);
    Task<List<CsiInspectionAssemblyDto>?> SaveForProfessionalAsync(int inspectionId, List<CsiInspectionAssemblyRequest> requests, CancellationToken cancellationToken);
    Task DeleteForInspectionAsync(int inspectionId, string? submissionId, CancellationToken cancellationToken);
}
