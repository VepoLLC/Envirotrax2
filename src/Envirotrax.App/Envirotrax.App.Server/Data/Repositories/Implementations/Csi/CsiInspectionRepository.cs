using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

public class CsiInspectionRepository : Repository<CsiInspection>, ICsiInspectionRepository
{
    public CsiInspectionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<CsiInspection> GetListQuery()
    {
        return base.GetListQuery()
            .Include(c => c.Site);
    }

    protected override IQueryable<CsiInspection> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(c => c.Site)
            .Include(c => c.WaterSupplier);
    }

    public async Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo,
        Query query,
        bool latestOnly,
        CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery()
            .Where(c => c.Site != null && !c.Site.OutOfArea)
            .Where(query.Filter);

        dbQuery = await ApplyLatestOnlyFilterAsync(dbQuery, latestOnly, cancellationToken);

        var paginated = await dbQuery
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    private static async Task<IQueryable<CsiInspection>> ApplyLatestOnlyFilterAsync(IQueryable<CsiInspection> query, bool latestOnly, CancellationToken cancellationToken)
    {
        if (!latestOnly)
        {
            return query;
        }

        var latestIds = await query
            .GroupBy(c => c.SiteId)
            .Select(g => g.OrderByDescending(c => c.InspectionDate).ThenByDescending(c => c.Id).First().Id)
            .ToListAsync(cancellationToken);

        return query.Where(c => latestIds.Contains(c.Id));
    }
}
