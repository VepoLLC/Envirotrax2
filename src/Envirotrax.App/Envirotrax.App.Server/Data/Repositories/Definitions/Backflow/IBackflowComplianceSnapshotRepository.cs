using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowComplianceSnapshotRepository
{
    Task<IEnumerable<BackflowComplianceSnapshot>> GetAllAsync(CancellationToken cancellationToken);

    Task<BackflowComplianceSnapshot> UpsertAsync(BackflowComplianceSnapshot snapshot, CancellationToken cancellationToken);

    Task BulkUpsertAsync(IEnumerable<BackflowComplianceSnapshot> snapshots, CancellationToken cancellationToken);
}
