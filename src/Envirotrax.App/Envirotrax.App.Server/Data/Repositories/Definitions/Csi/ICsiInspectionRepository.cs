using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionRepository : IRepository<CsiInspection>
{
    Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);
    Task<CsiInspection?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken);
}
