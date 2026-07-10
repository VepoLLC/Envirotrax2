using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowOutOfServiceRequestRepository : IRepository<BackflowOutOfServiceRequest>
{
    Task<IEnumerable<BackflowTest>> GetReplacementCandidatesAsync(int testId, CancellationToken cancellationToken);

    Task<bool> HasRequestForTestAsync(int testId, CancellationToken cancellationToken);

    Task<int?> GetTestWaterSupplierIdAsync(int testId, CancellationToken cancellationToken);
}
