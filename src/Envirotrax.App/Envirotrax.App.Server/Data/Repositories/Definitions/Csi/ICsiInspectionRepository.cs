using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionRepository : IRepository<CsiInspection>
{
    Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken);
}
