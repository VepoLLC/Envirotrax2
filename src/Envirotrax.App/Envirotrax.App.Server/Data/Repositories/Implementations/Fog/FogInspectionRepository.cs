using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogInspectionRepository : Repository<FogInspection>, IFogInspectionRepository
{
    public FogInspectionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<FogInspection> GetListQuery()
    {
        return base.GetListQuery()
            .Include(fi => fi.Site);
    }

    protected override IQueryable<FogInspection> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(fi => fi.Site)
            .Include(fi => fi.WaterSupplier)
            .ThenInclude(ws => ws!.State)
            .Include(fi => fi.Inspector)
            .Include(fi => fi.PropertyState)
            .Include(fi => fi.MailingState);
    }

    public override Task<IEnumerable<FogInspection>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogInspection.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
            query.Sort[nameof(FogInspection.Id)] = SortOperator.Asc;

        var filteredQ = GetListQuery()
            .Where(query.Filter);

        if (latestOnly)
        {
            var filteredInspections = filteredQ;
            filteredQ = filteredInspections.Where(fi =>
                !filteredInspections.Any(other =>
                    other.SiteId == fi.SiteId &&
                    (other.InspectionDate > fi.InspectionDate ||
                        (other.InspectionDate == fi.InspectionDate && other.Id > fi.Id))));
        }

        var paginated = await filteredQ
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
