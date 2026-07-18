using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
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

        await DbContext.SaveChangesAsync(CancellationToken.None);

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
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.ExpirationDate, expirationDate.Value), cancellationToken);
        }
        else
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired), cancellationToken);
        }
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
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.ExpirationDate, expirationDate.Value)
                    .SetProperty(x => x.NeedsRenewalCheck, false), cancellationToken);
        }
        else
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.NeedsRenewalCheck, false), cancellationToken);
        }
    }

    public async Task ClearTestNeedsRenewalCheckAsync(int testId, CancellationToken cancellationToken)
    {
        await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.NeedsRenewalCheck, false), CancellationToken.None);
    }

    public async Task<BackflowTest?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.RenewalRequired = renewalRequired;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.RenewalRequired).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateScheduleMonthAsync(int id, int month, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.BackflowScheduleMonth = month;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.BackflowScheduleMonth).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateIsCurrentAsync(int id, bool isCurrent, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.IsCurrent = isCurrent;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.IsCurrent).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateOutOfServiceAsync(int id, bool outOfService, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.OutOfService = outOfService;
        test.OutOfServiceDate = outOfService ? DateTime.UtcNow : null;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.OutOfService).IsModified = true;
        DbContext.Entry(test).Property(x => x.OutOfServiceDate).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        if (outOfService)
        {
            var settings = await DbContext.FindAsync<BackflowSettings>(test.WaterSupplierId, cancellationToken);

            if (settings?.OutOfServiceRequiresApproval == true)
            {
                test.Disapproved = true;
                DbContext.Entry(test).Property(x => x.Disapproved).IsModified = true;
            }
        }

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateDisapprovalAsync(int id, bool disapproved, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.Disapproved = disapproved;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.Disapproved).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        if (!disapproved)
        {
            test.ApprovalDate = DateTime.UtcNow;
            test.ApprovedById = updatedById;

            DbContext.Entry(test).Property(x => x.ApprovalDate).IsModified = true;
            DbContext.Entry(test).Property(x => x.ApprovedById).IsModified = true;

            if (test.OutOfServiceDate == null)
            {
                test.OutOfServiceDate = DateTime.UtcNow;
                DbContext.Entry(test).Property(x => x.OutOfServiceDate).IsModified = true;
            }
        }

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateForceRenewalAsync(int id, bool forceRenewal, int forceRenewalYears, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.ForceRenewal = forceRenewal;
        test.ForceRenewalYears = forceRenewalYears;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.ForceRenewal).IsModified = true;
        DbContext.Entry(test).Property(x => x.ForceRenewalYears).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        if (forceRenewal && test.IsCurrent && !test.OutOfService && test.TestResult == BackflowTestResult.Pass)
        {
            var baseDate = test.TestDate ?? DateTime.UtcNow;
            var newExpiration = forceRenewalYears == 0
                ? baseDate.AddMonths(6)
                : baseDate.AddYears(forceRenewalYears);

            test.ExpirationDate = newExpiration;
            DbContext.Entry(test).Property(x => x.ExpirationDate).IsModified = true;
        }

        await DbContext.SaveChangesAsync(CancellationToken.None);

        return test;
    }

    public async Task<BackflowTest?> UpdateRejectionAsync(int id, bool rejected, string? rejectedReason, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        test.Rejected = rejected;
        test.UpdatedById = updatedById;
        test.UpdatedTime = DateTime.UtcNow;

        DbContext.Entry(test).Property(x => x.Rejected).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedById).IsModified = true;
        DbContext.Entry(test).Property(x => x.UpdatedTime).IsModified = true;

        if (rejected)
        {
            test.IsCurrent = false;
            test.RejectedById = updatedById;
            test.RejectedDate = DateTime.UtcNow;
            test.RejectedReason = rejectedReason;

            DbContext.Entry(test).Property(x => x.IsCurrent).IsModified = true;
            DbContext.Entry(test).Property(x => x.RejectedById).IsModified = true;
            DbContext.Entry(test).Property(x => x.RejectedDate).IsModified = true;
            DbContext.Entry(test).Property(x => x.RejectedReason).IsModified = true;

            await DbContext.SaveChangesAsync(CancellationToken.None);

            var previousId = await FindPreviousTestIdAsync(test, cancellationToken);

            if (previousId.HasValue)
            {
                await DbContext.BackflowTests
                    .IgnoreQueryFilters()
                    .Where(t => t.Id == previousId.Value)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsCurrent, true), CancellationToken.None);
            }
        }
        else
        {
            await DbContext.SaveChangesAsync(CancellationToken.None);

            await ReassignIsCurrentForDeviceAsync(test, updatedById, cancellationToken);
        }

        return test;
    }

    private async Task ReassignIsCurrentForDeviceAsync(BackflowTest fromTest, int updatedById, CancellationToken cancellationToken)
    {
        if (fromTest.SiteId == null || string.IsNullOrWhiteSpace(fromTest.SerialNumber))
        {
            return;
        }

        var siteCandidates = await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => t.SiteId == fromTest.SiteId && t.DeletedTime == null && !t.Rejected)
            .Select(t => new { t.Id, t.SerialNumber, t.TestDate, t.IsCurrent })
            .ToListAsync(cancellationToken);

        var matchingTests = siteCandidates
            .Where(t => AreSerialNumbersMatching(t.SerialNumber, fromTest.SerialNumber))
            .ToList();

        if (matchingTests.Count == 0)
        {
            return;
        }

        var latestId = matchingTests.OrderByDescending(t => t.TestDate).First().Id;

        foreach (var match in matchingTests)
        {
            if (match.Id == latestId && !match.IsCurrent)
            {
                await DbContext.BackflowTests.IgnoreQueryFilters()
                    .Where(t => t.Id == match.Id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.IsCurrent, true)
                        .SetProperty(x => x.UpdatedById, updatedById)
                        .SetProperty(x => x.UpdatedTime, DateTime.UtcNow), CancellationToken.None);
            }
            else if (match.Id != latestId && match.IsCurrent)
            {
                await DbContext.BackflowTests.IgnoreQueryFilters()
                    .Where(t => t.Id == match.Id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.IsCurrent, false)
                        .SetProperty(x => x.UpdatedById, updatedById)
                        .SetProperty(x => x.UpdatedTime, DateTime.UtcNow), CancellationToken.None);
            }
        }
    }

    private static bool AreSerialNumbersMatching(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
        {
            return false;
        }

        if (string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return NormalizeSerialNumber(a) == NormalizeSerialNumber(b);
    }

    private static string NormalizeSerialNumber(string serial)
    {
        return string.Concat(serial.Where(char.IsDigit)).TrimStart('0');
    }

    private async Task<int?> FindPreviousTestIdAsync(BackflowTest fromTest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fromTest.SerialNumber))
        {
            return null;
        }

        var query = DbContext.BackflowTests
            .Where(t =>
                t.SerialNumber == fromTest.SerialNumber &&
                t.Id != fromTest.Id &&
                t.DeletedTime == null &&
                !t.Rejected &&
                t.CreatedTime < fromTest.CreatedTime);

        if (!string.IsNullOrWhiteSpace(fromTest.Manufacturer))
        {
            query = query.Where(t => t.Manufacturer == fromTest.Manufacturer);
        }

        var candidates = await query
            .OrderByDescending(t => t.CreatedTime)
            .Select(t => new { t.Id, t.SiteId, t.PropertyStreetNumber })
            .ToListAsync(cancellationToken);

        if (candidates.Count == 0)
        {
            return null;
        }

        if (candidates.Count == 1)
        {
            var only = candidates[0];
            var streetMatch = string.Equals(
                only.PropertyStreetNumber?.Trim(),
                fromTest.PropertyStreetNumber?.Trim(),
                StringComparison.OrdinalIgnoreCase);

            return (only.SiteId == fromTest.SiteId || streetMatch) ? only.Id : null;
        }

        // Multiple candidates — prefer SiteId match, then PropertyStreetNumber match.
        var bySite = candidates.Where(t => t.SiteId == fromTest.SiteId).ToList();
        var pool = bySite.Count > 0 ? bySite : candidates
            .Where(t => string.Equals(
                t.PropertyStreetNumber?.Trim(),
                fromTest.PropertyStreetNumber?.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        return pool.Count > 0 ? pool[0].Id : null;
    }
}
