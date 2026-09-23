using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;

public interface IBackflowTestRepository : IRepository<BackflowTest>
{
    Task<List<BackflowTest>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);

    // Resolved for the whole batch in one query, keyed by test id. Tests without an earlier
    // match are simply absent from the result.
    Task<Dictionary<int, BackflowTest>> FindPreviousTestsAsync(IReadOnlyCollection<BackflowTest> tests, CancellationToken cancellationToken);

    Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName);

    Task<BackflowTestExpiryCounts> GetExpiryCountsAsync(CancellationToken cancellationToken);

    // Compliance Management: current, in-service, renewal-required assemblies on active, in-area sites.
    Task<IEnumerable<BackflowTest>> GetComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<IEnumerable<BackflowTest>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    // Process 1 — Site level
    Task<IEnumerable<BackflowTest>> GetAllCurrentBySiteIdAsync(int siteId, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateTestRenewalAsync(int testId, bool renewalRequired, DateTime? expirationDate);

    // Process 2 — Test level
    Task<IEnumerable<BackflowTest>> GetAllPendingRenewalByTestFlagAsync(int batchSize, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateTestRenewalAndClearFlagAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken);
    Task ClearTestNeedsRenewalCheckAsync(int testId, CancellationToken cancellationToken);

    // Status updates
    Task<BackflowTest?> UpdateForAdminAsync(int id, BackflowTestAdminUpdateRequest request, int updatedById);

    Task<BackflowTest?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateScheduleMonthAsync(int id, int month, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateIsCurrentAsync(int id, bool isCurrent, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateOutOfServiceAsync(int id, bool outOfService, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateDisapprovalAsync(int id, bool disapproved, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateForceRenewalAsync(int id, bool forceRenewal, int forceRenewalYears, int updatedById, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateRejectionAsync(int id, bool rejected, string? rejectedReason, int updatedById, CancellationToken cancellationToken);

    // Professional edit-in-place (checkout "Edit" on an own, still-unpaid test)
    Task<BackflowTest?> UpdateForProfessionalAsync(
        BackflowTest model,
        int professionalId,
        string? newAssemblyImagePath,
        string? newSerialNumberImagePath,
        string? newBypassAssemblyImagePath,
        string? newBypassSerialNumberImagePath,
        string? newAirGapImagePath);

    Task<IEnumerable<BackflowTest>> GetReplacementsAsync(PageInfo pageInfo, Query query, bool onHold, CancellationToken cancellationToken);
    Task<BackflowTest?> GetReplacedAssemblyAsync(int id, CancellationToken cancellationToken);
    Task<BackflowTest?> UpdateReplacementHoldAsync(int id, bool onHold);
    Task<BackflowTest?> UpdateReplacementClearedAsync(int id, bool cleared);

    Task<int> CountCurrentInServiceBySiteAsync(int siteId, CancellationToken cancellationToken);

    // Checkout
    Task<List<BackflowTest>> GetUnpaidForCheckoutAsync(IReadOnlyCollection<int> ids, int professionalId, int? bpatId, CancellationToken cancellationToken);
    Task<int> MarkPaidAsync(IReadOnlyCollection<int> ids, int professionalId, int? bpatId, string transactionId, DateTime transactionDate, IReadOnlyCollection<int> emailPdfTestIds, CancellationToken cancellationToken);
    Task<decimal> SumAmountByTransactionIdAsync(string transactionId, CancellationToken cancellationToken);
    Task<List<BackflowTest>> GetByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken);
    Task<BackflowTest?> FindPreviousCurrentTestAsync(BackflowTest test, CancellationToken cancellationToken);
    Task SetIsCurrentAsync(int id, bool isCurrent, CancellationToken cancellationToken);
}
