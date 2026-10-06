using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionAssemblyRepository : IRepository<CsiInspectionVisuallyIdentifiedAssembly>
{
    Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<CsiInspectionVisuallyIdentifiedAssembly> assemblies, CancellationToken cancellationToken);
    Task UpdateVisuallyIdentifiedAsync(int inspectionId, int professionalId, IReadOnlyCollection<int> visuallyIdentifiedIds, CancellationToken cancellationToken);
    Task MarkPaidAsync(int inspectionId, int professionalId, string transactionId, CancellationToken cancellationToken);
    Task DeleteFromInspectionAsync(int inspectionId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
    Task DeleteByInspectionAsync(int inspectionId, int professionalId, CancellationToken cancellationToken);
}
