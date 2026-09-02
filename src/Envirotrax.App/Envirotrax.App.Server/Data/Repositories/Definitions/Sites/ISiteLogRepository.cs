using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

public interface ISiteLogRepository : IRepository<SiteLog>
{
    Task<IEnumerable<SiteLog>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<SiteLog>> GetBySiteIdsAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken);
}
