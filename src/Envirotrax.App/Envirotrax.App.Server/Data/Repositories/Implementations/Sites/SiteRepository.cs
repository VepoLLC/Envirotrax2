using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteRepository : Repository<Site>, ISiteRepository
{
    public SiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void UpdateEntity(Site model)
    {
        base.UpdateEntity(model);

        var entry = DbContext.Entry(model);

        entry.Property(site => site.GisAreaId).IsModified = false;
        entry.Property(site => site.GisDate).IsModified = false;
        entry.Property(site => site.GisLatitude).IsModified = false;
        entry.Property(site => site.GisLongitude).IsModified = false;
        entry.Property(site => site.GisStatus).IsModified = false;
    }

    protected override IQueryable<Site> GetListQuery()
    {
        return base.GetListQuery()
            .Include(s => s.UpdatedBy)
            .Include(s => s.WaterSupplier)
            .Include(s => s.State)
            .Include(s => s.MailingState)
            .AsNoTracking();
    }

    public async Task<IEnumerable<Site>> SearchAsync(PageInfo pageInfo, Query query, bool? fogCompliant, CancellationToken cancellationToken)
    {
        var sites = GetListQuery().Where(query.Filter);

        if (fogCompliant.HasValue)
        {
            var now = DateTime.UtcNow;

            if (fogCompliant.Value)
            {
                sites = sites.Where(s => s.TripTicketInterval > 0 && s.LastTripTicketDate != null && s.LastTripTicketDate.Value.AddDays(s.TripTicketInterval) >= now);
            }
            else
            {
                sites = sites.Where(s => s.TripTicketInterval > 0 && s.LastTripTicketDate != null && s.LastTripTicketDate.Value.AddDays(s.TripTicketInterval) < now);
            }
        }

        var paginated = await sites
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    protected override IQueryable<Site> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(s => s.State)
            .Include(s => s.MailingState)
            .Include(s => s.WaterSupplier).ThenInclude(ws => ws!.State);
    }

    public async Task<IEnumerable<Site>> GetAllPendingGeocodingAsync(int batchSize)
    {
        var thirthyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return await DbContext
            .Sites
            .IgnoreQueryFilters()
            .Where(site => site.DeletedTime == null && site.GisStatus == GisStatusType.NotSet || (site.GisStatus == GisStatusType.Error && site.GisDate < thirthyDaysAgo))
            .OrderBy(site => site.GisDate)
            .Take(batchSize)
            .Select(s => new Site
            {
                WaterSupplierId = s.WaterSupplierId,
                Id = s.Id
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateGisCoordinatesAsync(Site site)
    {
        await DbContext
            .Sites
            .Where(s => s.Id == site.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(site => site.GisLongitude, site.GisLongitude)
                .SetProperty(site => site.GisLatitude, site.GisLatitude)
                .SetProperty(site => site.GisDate, site.GisDate)
                .SetProperty(site => site.GisStatus, site.GisStatus)
                .SetProperty(site => site.GisAreaId, site.GisAreaId));
    }

    public async Task UpdateManualGisDataAsync(int siteId, double? latitude, double? longitude, GisStatusType status)
    {
        await DbContext
            .Sites
            .Where(s => s.Id == siteId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(s => s.GisLatitude, latitude)
                .SetProperty(s => s.GisLongitude, longitude)
                .SetProperty(s => s.GisStatus, status));
    }

    public async Task<IEnumerable<Site>> GetCsiComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await GetListQuery()
            .Where(s => s.NeedsCsiInspection && !s.OutOfArea)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task UpdateCsiAssignmentAsync(int siteId, int? userId, DateTime? assignmentDate)
    {
        await DbContext
            .Sites
            .Where(s => s.Id == siteId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(s => s.CsiAccountAssignmentId, userId)
                .SetProperty(s => s.CsiAccountAssignmentDate, assignmentDate));
    }
}
