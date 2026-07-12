using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName);

    Task<BackflowTestExpiryCounts> GetExpiryCountsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<BackflowTest>> GetAllPendingRenewalAsync(int batchSize, CancellationToken cancellationToken);
}
