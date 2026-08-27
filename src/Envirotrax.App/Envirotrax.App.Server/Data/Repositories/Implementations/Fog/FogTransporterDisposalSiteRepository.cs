using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogTransporterDisposalSiteRepository : Repository<FogTransporterDisposalSite>, IFogTransporterDisposalSiteRepository
{
    public FogTransporterDisposalSiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<FogDisposalSite>> GetRegisteredDisposalSitesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogDisposalSite.County)] = SortOperator.Asc;
        }

        var paginated = await GetRegisteredDisposalSitesQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<int> CountRegisteredDisposalSitesAsync(Query query, CancellationToken cancellationToken)
    {
        return GetRegisteredDisposalSitesQuery()
            .Where(query.Filter)
            .CountAsync(cancellationToken);
    }

    private IQueryable<FogDisposalSite> GetRegisteredDisposalSitesQuery()
    {
        return Entity
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Select(r => r.DisposalSite!)
            .Where(s => s.DeletedTime == null);
    }

    // A professional links a disposal site at most once (composite PK), so there is a single registration
    // row per site. Add it if it doesn't exist, otherwise update its IsActive flag.
    public async Task<FogTransporterDisposalSite> SetRegistrationAsync(FogTransporterDisposalSite registration, CancellationToken cancellationToken)
    {
        var existing = await Entity
            .SingleOrDefaultAsync(r => r.DisposalSiteId == registration.DisposalSiteId, cancellationToken);

        if (existing == null)
        {
            Entity.Add(registration);
            await DbContext.SaveChangesAsync();
            return registration;
        }

        existing.IsActive = registration.IsActive;
        await DbContext.SaveChangesAsync();
        return existing;
    }
}
