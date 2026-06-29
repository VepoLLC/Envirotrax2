using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogDisposalSiteRepository : Repository<FogDisposalSite>, IFogDisposalSiteRepository
{
    public FogDisposalSiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<FogDisposalSite> GetListQuery()
    {
        return base.GetListQuery().Where(s => s.DeletedTime == null);
    }

    protected override IQueryable<FogDisposalSite> GetDetailsQuery()
    {
        return base.GetDetailsQuery().Where(s => s.DeletedTime == null);
    }

    public async Task<IEnumerable<FogDisposalSite>> GetActiveAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogDisposalSite.County)] = SortOperator.Asc;
        }

        var paginated = await GetListQuery()
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
