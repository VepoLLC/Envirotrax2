using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogDisposalSiteRepository : IRepository<FogDisposalSite>
{
    Task<IEnumerable<FogDisposalSite>> GetActiveAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
