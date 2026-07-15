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
            .Include(bt => bt.WaterSupplier)
            .Include(bt => bt.Site)
            .Include(bt => bt.Bpat)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState);
    }

    protected override IQueryable<BackflowTest> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(bt => bt.WaterSupplier)
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

    // Image paths are owned by the dedicated image flow (UpdateImagePathAsync),
    // so a regular update must never overwrite them.
    protected override void UpdateEntity(BackflowTest model)
    {
        base.UpdateEntity(model);

        var entry = DbContext.Entry(model);
        entry.Property(m => m.AssemblyImagePath).IsModified = false;
        entry.Property(m => m.SerialNumberImagePath).IsModified = false;
        entry.Property(m => m.BypassAssemblyImagePath).IsModified = false;
        entry.Property(m => m.BypassSerialNumberImagePath).IsModified = false;
        entry.Property(m => m.AirGapImagePath).IsModified = false;
    }

    public async Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName)
    {
        DbContext.Attach(model);
        DbContext.Entry(model).Property(imagePathPropertyName).IsModified = true;

        await DbContext.SaveChangesAsync();

        return model;
    }

    public async Task<BackflowTestExpiryCounts> GetExpiryCountsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var expiredStart = now.AddMonths(-6);
        var thisMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = thisMonthStart.AddMonths(1);
        var twoMonthsStart = thisMonthStart.AddMonths(2);
        var threeMonthsStart = thisMonthStart.AddMonths(3);

        var counts = await Entity
            .Where(t => t.IsCurrent)
            .GroupBy(t => 1)
            .Select(g => new BackflowTestExpiryCounts
            {
                Expired = g.Count(t => t.ExpirationDate >= expiredStart && t.ExpirationDate <= now),
                ThisMonth = g.Count(t => t.ExpirationDate >= thisMonthStart && t.ExpirationDate < nextMonthStart),
                NextMonth = g.Count(t => t.ExpirationDate >= nextMonthStart && t.ExpirationDate < twoMonthsStart),
                TwoMonths = g.Count(t => t.ExpirationDate >= twoMonthsStart && t.ExpirationDate < threeMonthsStart)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return counts ?? new BackflowTestExpiryCounts();
    }

    public async Task<IEnumerable<BackflowTest>> GetAllCurrentBySiteIdAsync(int siteId, CancellationToken cancellationToken)
    {
        return await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => t.DeletedTime == null && t.IsCurrent && t.SiteId == siteId)
            .Select(t => new BackflowTest
            {
                Id = t.Id,
                PropertyType = t.PropertyType,
                DeviceType = t.DeviceType,
                HazardType = t.HazardType,
                Ossf = t.Ossf,
                TestDate = t.TestDate,
                TestResult = t.TestResult,
                OutOfService = t.OutOfService,
                Site = t.Site == null ? null : new Site { HasAuxWaterSupply = t.Site.HasAuxWaterSupply }
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTestRenewalAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken)
    {
        if (expirationDate.HasValue)
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.ExpirationDate, expirationDate.Value), cancellationToken);
        else
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired), cancellationToken);
    }

    public async Task<IEnumerable<BackflowTest>> GetAllPendingRenewalByTestFlagAsync(int batchSize, CancellationToken cancellationToken)
    {
        return await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => t.DeletedTime == null && t.NeedsRenewalCheck && t.IsCurrent)
            .OrderBy(t => t.Id)
            .Take(batchSize)
            .Select(t => new BackflowTest
            {
                Id = t.Id,
                WaterSupplierId = t.WaterSupplierId,
                PropertyType = t.PropertyType,
                DeviceType = t.DeviceType,
                HazardType = t.HazardType,
                Ossf = t.Ossf,
                TestDate = t.TestDate,
                TestResult = t.TestResult,
                OutOfService = t.OutOfService,
                Site = t.Site == null ? null : new Site { HasAuxWaterSupply = t.Site.HasAuxWaterSupply }
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTestRenewalAndClearFlagAsync(int testId, bool renewalRequired, DateTime? expirationDate, CancellationToken cancellationToken)
    {
        if (expirationDate.HasValue)
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.ExpirationDate, expirationDate.Value)
                    .SetProperty(x => x.NeedsRenewalCheck, false), cancellationToken);
        else
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.NeedsRenewalCheck, false), cancellationToken);
    }

    public async Task ClearTestNeedsRenewalCheckAsync(int testId, CancellationToken cancellationToken)
    {
        await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.NeedsRenewalCheck, false), cancellationToken);
    }
}
