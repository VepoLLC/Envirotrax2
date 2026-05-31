using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowTestRepository : Repository<BackflowTest>, IBackflowTestRepository
{
    public BackflowTestRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<BackflowTest> GetListQuery()
    {
        return base.GetListQuery()
            .Include(bt => bt.Site)
            .Include(bt => bt.Bpat)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState);
    }

    protected override IQueryable<BackflowTest> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(bt => bt.Site)
            .Include(bt => bt.Bpat)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState);
    }

    public override Task<IEnumerable<BackflowTest>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<BackflowTest>> GetAllAsync(PageInfo pageInfo, Query query, int? gisAreaId, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }

        var query2 = GetListQuery();
        if (gisAreaId.HasValue)
        {
            query2 = query2.Where(bt => bt.Site != null && bt.Site.GisAreaId == gisAreaId.Value);
        }

        var paginated = await query2
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
