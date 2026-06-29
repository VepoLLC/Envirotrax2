using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteLogRepository : Repository<SiteLog>, ISiteLogRepository
{
    public SiteLogRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<SiteLog> GetListQuery()
    {
        return base.GetListQuery()
            .Include(sl => sl.CreatedBy)
            .Include(sl => sl.Assembly);
    }

    protected override IQueryable<SiteLog> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(sl => sl.CreatedBy)
            .Include(sl => sl.Assembly);
    }

    public override Task<IEnumerable<SiteLog>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(SiteLog.Id)] = SortOperator.Desc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<SiteLog>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(SiteLog.Id)] = SortOperator.Desc;
        }

        var paginated = await GetListQuery()
            .Where(sl => sl.SiteId == siteId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
