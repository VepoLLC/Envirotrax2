using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTransporterDisposalSiteRepository : IRepository<FogTransporterDisposalSite>
{
    Task<IEnumerable<FogDisposalSite>> GetRegisteredDisposalSitesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken);
}
