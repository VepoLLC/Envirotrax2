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

        var sites = Entity
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Select(r => r.DisposalSite!)
            .Where(s => s.DeletedTime == null);

        var paginated = await sites
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken)
    {
        var existing = await Entity
            .Where(r => r.DisposalSiteId == disposalSiteId)
            .ToListAsync(cancellationToken);

        if (isActive)
        {
            if (!existing.Any(r => r.IsActive))
            {
                Entity.Add(new FogTransporterDisposalSite
                {
                    DisposalSiteId = disposalSiteId,
                    IsActive = true
                });
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }
        else if (existing.Count > 0)
        {
            DbContext.RemoveRange(existing);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
