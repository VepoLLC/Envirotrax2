using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogTransporterDisposalSiteRepository : IRepository<FogTransporterDisposalSite>
{
    Task<IEnumerable<FogDisposalSiteCandidate>> GetAvailableAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogDisposalSiteCandidate> SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken);
}
