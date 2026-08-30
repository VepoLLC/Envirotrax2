using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionRepository : IRepository<CsiInspection>
{
    Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);
    Task<IEnumerable<CsiInspection>> SearchForAdminAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    // Dashboard "View" on a sub account: shows that child water supplier's own inspections without
    // switching the current session's authentication.
    Task<IEnumerable<CsiInspection>> SearchForSubAccountAsync(PageInfo pageInfo, Query query, int subAccountWaterSupplierId, CancellationToken cancellationToken);
    Task<CsiInspection?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken);
    Task<AdminUpdateResult<CsiInspection>> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request);
}
