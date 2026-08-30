using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName);

    Task<BackflowTestExpiryCounts> GetExpiryCountsAsync(CancellationToken cancellationToken);

    // Compliance Management: current, in-service, renewal-required assemblies on active, in-area sites.
    Task<IEnumerable<BackflowTest>> GetComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<IEnumerable<BackflowTest>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    // Dashboard "View" on a sub account: shows that child water supplier's own tests without
    // switching the current session's authentication.
    Task<IEnumerable<BackflowTest>> SearchForSubAccountAsync(PageInfo pageInfo, Query query, int subAccountWaterSupplierId, CancellationToken cancellationToken);

    // Process 1 — Site level
    Task<IEnumerable<BackflowTest>> GetAllCurrentBySiteIdAsync(int siteId, CancellationToken cancellationToken);
    Task UpdateTestRenewalAsync(int testId, bool renewalRequired, DateTime? expirationDate);

    // Process 2 — Test level
    Task<IEnumerable<BackflowTest>> GetAllPendingRenewalByTestFlagAsync(int batchSize, CancellationToken cancellationToken);
    Task UpdateTestRenewalAndClearFlagAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken);
    Task ClearTestNeedsRenewalCheckAsync(int testId, CancellationToken cancellationToken);

    // Status updates
    Task<AdminUpdateResult<BackflowTest>> UpdateForAdminAsync(int id, BackflowTestAdminUpdateRequest request, int updatedById);

    Task<BackflowTest?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateScheduleMonthAsync(int id, int month, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateIsCurrentAsync(int id, bool isCurrent, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateOutOfServiceAsync(int id, bool outOfService, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateDisapprovalAsync(int id, bool disapproved, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateForceRenewalAsync(int id, bool forceRenewal, int forceRenewalYears, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateRejectionAsync(int id, bool rejected, string? rejectedReason, int updatedById, CancellationToken cancellationToken);
}
