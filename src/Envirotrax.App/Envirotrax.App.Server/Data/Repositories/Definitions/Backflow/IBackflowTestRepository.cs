using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName);

    Task<BackflowTestExpiryCounts> GetExpiryCountsAsync(CancellationToken cancellationToken);

    // Process 1 — Site level
    Task<IEnumerable<BackflowTest>> GetAllCurrentBySiteIdAsync(int siteId, CancellationToken cancellationToken);
    Task UpdateTestRenewalAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken);

    // Process 2 — Test level
    Task<IEnumerable<BackflowTest>> GetAllPendingRenewalByTestFlagAsync(int batchSize, CancellationToken cancellationToken);
    Task UpdateTestRenewalAndClearFlagAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken);
    Task ClearTestNeedsRenewalCheckAsync(int testId, CancellationToken cancellationToken);
}
