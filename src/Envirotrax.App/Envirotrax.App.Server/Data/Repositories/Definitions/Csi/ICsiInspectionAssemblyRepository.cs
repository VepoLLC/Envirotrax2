using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionAssemblyRepository : IRepository<CsiInspectionVisuallyIdentifiedAssembly>
{
    Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetBySubmissionAsync(string submissionId, CancellationToken cancellationToken);
    Task<bool> DeleteForSubmissionAsync(int id, string submissionId, CancellationToken cancellationToken);
    Task UpdateVisuallyIdentifiedAsync(string submissionId, IReadOnlyCollection<int> visuallyIdentifiedIds, CancellationToken cancellationToken);
    Task LinkToInspectionAsync(int inspectionId, string submissionId, CancellationToken cancellationToken);
    Task MarkPaidAsync(int inspectionId, string submissionId, string transactionId, CancellationToken cancellationToken);
    Task DeleteByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
}
