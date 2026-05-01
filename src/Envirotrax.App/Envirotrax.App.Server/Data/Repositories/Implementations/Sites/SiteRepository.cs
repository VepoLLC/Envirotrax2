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
            .Include(s => s.State)
            .Include(s => s.MailingState);
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
            .Where(site => site.GisStatus == GisStatusType.NotSet || (site.GisStatus == GisStatusType.Error && site.GisDate < thirthyDaysAgo))
            .OrderBy(site => site.GisDate)
            .Take(batchSize)
            .AsNoTracking()
            .ToListAsync();
    }
}
