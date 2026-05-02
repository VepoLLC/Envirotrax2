using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
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
            .Include(c => c.Site);
    }

    public async Task<CsiInspection?> GetForProfessionalAsync(int id, int professionalId, CancellationToken cancellationToken)
    {
        return await GetDetailsQuery()
            .Include(c => c.WaterSupplier)
            .Where(c => c.Id == id && c.ProfessionalId == professionalId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(
        int professionalId,
        PageInfo pageInfo,
        Query query,
        CsiInspectionProfessionalSearchRequest request,
        CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery()
            .Where(c => c.ProfessionalId == professionalId)
            .Where(c => c.Site != null && !c.Site.OutOfArea)
            .Where(query.Filter);

        dbQuery = ApplyDateFilter(dbQuery, request);
        dbQuery = await ApplyLatestOnlyFilterAsync(dbQuery, request, cancellationToken);
        dbQuery = ApplyPassFailFilter(dbQuery, request);

          var paginated = await dbQuery
                .OrderBy(query.Sort)
                .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    private static IQueryable<CsiInspection> ApplyDateFilter(IQueryable<CsiInspection> query, CsiInspectionProfessionalSearchRequest request)
    {
        if (request.DateType == CsiInspectionDateType.Inspection)
        {
            if (request.FromDate.HasValue)
            {
                query = query.Where(c => c.InspectionDate >= request.FromDate.Value.Date);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(c => c.InspectionDate < request.ToDate.Value.Date.AddDays(1));
            }
        }
        else if (request.DateType == CsiInspectionDateType.Submission)
        {
            if (request.FromDate.HasValue)
            {
                query = query.Where(c => c.CreatedTime >= request.FromDate.Value.Date);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(c => c.CreatedTime < request.ToDate.Value.Date.AddDays(1));
            }
        }

        return query;
    }

    private static IQueryable<CsiInspection> ApplyPassFailFilter(IQueryable<CsiInspection> query, CsiInspectionProfessionalSearchRequest request)
    {
        if (request.PassFail == CsiInspectionResultFilter.Pass)
        {
            query = query.Where(c => c.Compliance1 && c.Compliance2 && c.Compliance3 && c.Compliance4 && c.Compliance5 && c.Compliance6);
        }
        else if (request.PassFail == CsiInspectionResultFilter.Fail)
        {
            query = query.Where(c => !(c.Compliance1 && c.Compliance2 && c.Compliance3 && c.Compliance4 && c.Compliance5 && c.Compliance6));
        }

        return query;
    }

    private static async Task<IQueryable<CsiInspection>> ApplyLatestOnlyFilterAsync(IQueryable<CsiInspection> query, CsiInspectionProfessionalSearchRequest request, CancellationToken cancellationToken)
    {
        if (!request.LatestOnly)
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
