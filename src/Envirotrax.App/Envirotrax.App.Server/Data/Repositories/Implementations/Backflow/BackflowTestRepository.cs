using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowTestRepository : Repository<BackflowTest>, IBackflowTestRepository
{
    private const string HazardTypeOther = "Other";

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
                .ThenInclude(ws => ws!.State)
            .Include(bt => bt.Site)
            .Include(bt => bt.Professional)
            .Include(bt => bt.Bpat)
                .ThenInclude(bpat => bpat!.User)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState)
            .Include(bt => bt.ApprovedBy)
            .Include(bt => bt.RejectedBy);
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

    public async Task<IEnumerable<BackflowTest>> GetComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.ExpirationDate)] = SortOperator.Asc;
        }

        var paginated = await GetListQuery()
            .Where(t => t.IsCurrent
                && !t.OutOfService
                && t.RenewalRequired
                && t.Site != null
                && t.Site.Active
                && !t.Site.OutOfArea)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BackflowTest>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery().Where(query.Filter);

        if (paymentStatus == BackflowPaymentStatus.Paid)
        {
            dbQuery = dbQuery.Where(t => t.TransactionId != null && t.TransactionId != string.Empty);
        }

        if (paymentStatus == BackflowPaymentStatus.Unpaid)
        {
            dbQuery = dbQuery.Where(t => t.TransactionId == null || t.TransactionId == string.Empty);
        }

        var paginated = await dbQuery
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
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
                ExpirationDate = t.ExpirationDate,
                Site = t.Site == null ? null : new Site { HasAuxWaterSupply = t.Site.HasAuxWaterSupply }
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateTestRenewalAsync(int testId, bool renewalRequired, DateTime? expirationDate)
    {
        if (expirationDate.HasValue)
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired)
                    .SetProperty(x => x.ExpirationDate, expirationDate.Value));
        }
        else
        {
            await DbContext.BackflowTests.IgnoreQueryFilters().Where(t => t.Id == testId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.RenewalRequired, renewalRequired));
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

    public async Task<AdminUpdateResult<BackflowTest>> UpdateForAdminAsync(int id, BackflowTestAdminUpdateRequest request, int updatedById)
    {
        var result = new AdminUpdateResult<BackflowTest>();

        var test = await Entity.SingleOrDefaultAsync(t => t.Id == id);

        if (test == null)
        {
            return result;
        }

        var wasRejected = test.Rejected;
        var wasDisapproved = test.Disapproved;
        var wasForceRenewal = test.ForceRenewal;
        var renewalCheckNeeded = test.DeviceType != request.DeviceType || test.HazardType != request.HazardType;

        ApplyAdminEditableFields(test, request);

        await ApplyOutOfServiceAsync(test, request.OutOfService);

        var actingUserId = await ResolveWaterSupplierUserIdAsync(test.WaterSupplierId, updatedById);

        ApplyDisapproval(test, request.Disapproved, wasDisapproved, actingUserId);
        ApplyRejection(test, request.Rejected, wasRejected, actingUserId);
        ApplyForceRenewal(test, request.ForceRenewal, wasForceRenewal, request.ForceRenewalYears);

        if (renewalCheckNeeded)
        {
            test.NeedsRenewalCheck = true;
        }

        result.Changes = BuildChangeDescription(test);

        await DbContext.SaveChangesAsync();

        if (test.Rejected && !wasRejected)
        {
            var previousId = await FindPreviousTestIdAsync(test);

            if (previousId.HasValue)
            {
                await DbContext.BackflowTests
                    .IgnoreQueryFilters()
                    .Where(t => t.Id == previousId.Value)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsCurrent, true));
            }
        }

        if (!test.Rejected && wasRejected)
        {
            await ReassignIsCurrentForDeviceAsync(test, CancellationToken.None);
        }

        result.Model = test;

        return result;
    }

    private static void ApplyAdminEditableFields(BackflowTest test, BackflowTestAdminUpdateRequest request)
    {
        test.IsCurrent = request.IsCurrent;
        test.NeedsValidation = request.NeedsValidation;
        test.ValidationNotes = request.ValidationNotes;
        test.RenewalRequired = request.RenewalRequired;
        test.BackflowScheduleMonth = request.BackflowScheduleMonth;
        test.ForceRenewalYears = request.ForceRenewalYears;

        test.TestDate = request.TestDate;
        test.ExpirationDate = request.ExpirationDate;

        test.TransactionId = request.TransactionId;
        test.TransactionDate = request.TransactionDate;
        test.Amount = request.Amount;
        test.AmountShare = request.AmountShare;

        test.PropertyType = request.PropertyType;
        test.PropertyBusinessName = request.PropertyBusinessName;
        test.PropertyStreetNumber = request.PropertyStreetNumber;
        test.PropertyStreetName = request.PropertyStreetName;
        test.PropertyNumber = request.PropertyNumber;
        test.PropertyCity = request.PropertyCity;
        test.PropertyStateId = request.PropertyState?.Id;
        test.PropertyZip = request.PropertyZip;

        test.MailingCompanyName = request.MailingCompanyName;
        test.MailingContactName = request.MailingContactName;
        test.MailingStreetNumber = request.MailingStreetNumber;
        test.MailingStreetName = request.MailingStreetName;
        test.MailingNumber = request.MailingNumber;
        test.MailingCity = request.MailingCity;
        test.MailingStateId = request.MailingState?.Id;
        test.MailingZip = request.MailingZip;

        test.DeviceType = request.DeviceType;
        test.Manufacturer = request.Manufacturer;
        test.Model = request.Model;
        test.Size = request.Size;
        test.SerialNumber = request.SerialNumber;
        test.Manufacturer2 = request.Manufacturer2;
        test.Model2 = request.Model2;
        test.Size2 = request.Size2;
        test.SerialNumber2 = request.SerialNumber2;
        test.LocationDescription = request.LocationDescription;
        test.HazardType = request.HazardType;
        test.HazardTypeOtherDescription = request.HazardType == HazardTypeOther ? request.HazardTypeOtherDescription : null;

        test.TestResult = request.TestResult;
        test.JobNumber = request.JobNumber;
        test.ReasonForTest = request.ReasonForTest;
        test.ReplacementAssembly = request.ReplacementAssembly;
        test.ProperlyInstalled = request.ProperlyInstalled;
        test.NonPotable = request.NonPotable;

        test.InitialTestDate = request.InitialTestDate;
        test.InitCV1HeldPSID = request.InitCV1HeldPSID;
        test.InitCV1ClosedTight = request.InitCV1ClosedTight;
        test.InitCV1Leaked = request.InitCV1Leaked;
        test.InitCV2HeldPSID = request.InitCV2HeldPSID;
        test.InitCV2ClosedTight = request.InitCV2ClosedTight;
        test.InitCV2Leaked = request.InitCV2Leaked;
        test.InitRVOpenedPSID = request.InitRVOpenedPSID;
        test.InitRVDidNotOpen = request.InitRVDidNotOpen;
        test.InitBCHeldPSID = request.InitBCHeldPSID;
        test.InitBCClosedTight = request.InitBCClosedTight;
        test.InitBCLeaked = request.InitBCLeaked;
        test.InitPvbAirInletOpenedPSID = request.InitPvbAirInletOpenedPSID;
        test.InitPvbAirInletDidNotOpen = request.InitPvbAirInletDidNotOpen;
        test.InitPvbAirInletFullyOpened = request.InitPvbAirInletFullyOpened;
        test.InitPvbCVHeldPSID = request.InitPvbCVHeldPSID;
        test.InitPvbCVLeaked = request.InitPvbCVLeaked;

        test.InitCV1HeldPSID2 = request.InitCV1HeldPSID2;
        test.InitCV1ClosedTight2 = request.InitCV1ClosedTight2;
        test.InitCV1Leaked2 = request.InitCV1Leaked2;
        test.InitCV2HeldPSID2 = request.InitCV2HeldPSID2;
        test.InitCV2ClosedTight2 = request.InitCV2ClosedTight2;
        test.InitCV2Leaked2 = request.InitCV2Leaked2;
        test.InitRVOpenedPSID2 = request.InitRVOpenedPSID2;
        test.InitRVDidNotOpen2 = request.InitRVDidNotOpen2;

        test.RepairCV1Details = request.RepairCV1Details;
        test.RepairCV2Details = request.RepairCV2Details;
        test.RepairRVDetails = request.RepairRVDetails;
        test.RepairBCDetails = request.RepairBCDetails;
        test.RepairCV1Details2 = request.RepairCV1Details2;
        test.RepairCV2Details2 = request.RepairCV2Details2;
        test.RepairRVDetails2 = request.RepairRVDetails2;
        test.RepairPvbAirInletDetails = request.RepairPvbAirInletDetails;
        test.RepairPvbCVDetails = request.RepairPvbCVDetails;

        test.RepairTestDate = request.RepairTestDate;
        test.FinalCV1HeldPSID = request.FinalCV1HeldPSID;
        test.FinalCV1ClosedTight = request.FinalCV1ClosedTight;
        test.FinalCV2HeldPSID = request.FinalCV2HeldPSID;
        test.FinalCV2ClosedTight = request.FinalCV2ClosedTight;
        test.FinalRVOpenedPSID = request.FinalRVOpenedPSID;
        test.FinalBCHeldPSID = request.FinalBCHeldPSID;
        test.FinalBCClosedTight = request.FinalBCClosedTight;
        test.FinalPvbAirInletOpenedPSID = request.FinalPvbAirInletOpenedPSID;
        test.FinalPvbAirInletFullyOpened = request.FinalPvbAirInletFullyOpened;
        test.FinalPvbCVHeldPSID = request.FinalPvbCVHeldPSID;

        test.FinalCV1HeldPSID2 = request.FinalCV1HeldPSID2;
        test.FinalCV1ClosedTight2 = request.FinalCV1ClosedTight2;
        test.FinalCV2HeldPSID2 = request.FinalCV2HeldPSID2;
        test.FinalCV2ClosedTight2 = request.FinalCV2ClosedTight2;
        test.FinalRVOpenedPSID2 = request.FinalRVOpenedPSID2;

        test.MeterNumber = request.MeterNumber;
        test.MeterRegisters = request.MeterRegisters;
        test.MeterReadingBefore = request.MeterReadingBefore;
        test.MeterReadingAfter = request.MeterReadingAfter;

        test.AirGapValid = request.AirGapValid;

        test.Ossf = request.Ossf;
        test.RainFreezeSensorInstalled = request.RainFreezeSensorInstalled;
        test.RainFreezeSensorWorkingProperly = request.RainFreezeSensorWorkingProperly;
        test.PermitNumber = request.PermitNumber;

        test.Comments = request.Comments;
    }

    private async Task ApplyOutOfServiceAsync(BackflowTest test, bool outOfService)
    {
        if (test.OutOfService == outOfService)
        {
            return;
        }

        test.OutOfService = outOfService;
        test.OutOfServiceDate = outOfService ? DateTime.UtcNow : null;

        if (!outOfService)
        {
            return;
        }

        var settings = await DbContext.Set<BackflowSettings>()
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(s => s.WaterSupplierId == test.WaterSupplierId);

        if (settings?.OutOfServiceRequiresApproval == true)
        {
            test.Disapproved = true;
        }
    }

    private static void ApplyDisapproval(BackflowTest test, bool disapproved, bool wasDisapproved, int? actingUserId)
    {
        test.Disapproved = disapproved;

        if (disapproved || disapproved == wasDisapproved)
        {
            return;
        }

        test.ApprovalDate = DateTime.UtcNow;
        test.ApprovedById = actingUserId;

        if (test.OutOfServiceDate == null)
        {
            test.OutOfServiceDate = DateTime.UtcNow;
        }
    }

    private static void ApplyRejection(BackflowTest test, bool rejected, bool wasRejected, int? actingUserId)
    {
        test.Rejected = rejected;

        if (!rejected || rejected == wasRejected)
        {
            return;
        }

        test.IsCurrent = false;
        test.RejectedById = actingUserId;
        test.RejectedDate = DateTime.UtcNow;
    }

    private static void ApplyForceRenewal(BackflowTest test, bool forceRenewal, bool wasForceRenewal, int forceRenewalYears)
    {
        test.ForceRenewal = forceRenewal;

        if (!forceRenewal || forceRenewal == wasForceRenewal)
        {
            return;
        }

        if (!test.IsCurrent || test.OutOfService || test.TestResult != BackflowTestResult.Pass)
        {
            return;
        }

        var baseDate = test.TestDate ?? DateTime.UtcNow;

        test.ExpirationDate = forceRenewalYears == 0
            ? baseDate.AddMonths(6)
            : baseDate.AddYears(forceRenewalYears);
    }

    private async Task<int?> ResolveWaterSupplierUserIdAsync(int waterSupplierId, int userId)
    {
        if (userId <= 0)
        {
            return null;
        }

        var exists = await DbContext.Set<WaterSupplierUser>()
            .IgnoreQueryFilters()
            .AnyAsync(u => u.WaterSupplierId == waterSupplierId && u.UserId == userId);

        return exists ? userId : null;
    }

    public async Task<BackflowTest?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.RenewalRequired = renewalRequired;

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateScheduleMonthAsync(int id, int month, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.BackflowScheduleMonth = month;

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateIsCurrentAsync(int id, bool isCurrent, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.IsCurrent = isCurrent;

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateOutOfServiceAsync(int id, bool outOfService, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.OutOfService = outOfService;
        test.OutOfServiceDate = outOfService ? DateTime.UtcNow : null;

        if (outOfService)
        {
            var settings = await DbContext.FindAsync<BackflowSettings>(test.WaterSupplierId, cancellationToken);

            if (settings?.OutOfServiceRequiresApproval == true)
            {
                test.Disapproved = true;
            }
        }

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateDisapprovalAsync(int id, bool disapproved, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.Disapproved = disapproved;

        if (!disapproved)
        {
            test.ApprovalDate = DateTime.UtcNow;
            test.ApprovedById = updatedById;

            if (test.OutOfServiceDate == null)
            {
                test.OutOfServiceDate = DateTime.UtcNow;
            }
        }

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateForceRenewalAsync(int id, bool forceRenewal, int forceRenewalYears, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.ForceRenewal = forceRenewal;
        test.ForceRenewalYears = forceRenewalYears;

        if (forceRenewal && test.IsCurrent && !test.OutOfService && test.TestResult == BackflowTestResult.Pass)
        {
            var baseDate = test.TestDate ?? DateTime.UtcNow;
            var newExpiration = forceRenewalYears == 0
                ? baseDate.AddMonths(6)
                : baseDate.AddYears(forceRenewalYears);

            test.ExpirationDate = newExpiration;
        }

        await DbContext.SaveChangesAsync();

        return test;
    }

    public async Task<BackflowTest?> UpdateRejectionAsync(int id, bool rejected, string? rejectedReason, int updatedById, CancellationToken cancellationToken)
    {
        var test = await GetNoIncludesAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        DbContext.Attach(test);
        test.Rejected = rejected;

        if (rejected)
        {
            test.IsCurrent = false;
            test.RejectedById = updatedById;
            test.RejectedDate = DateTime.UtcNow;
            test.RejectedReason = rejectedReason;

            await DbContext.SaveChangesAsync();

            var previousId = await FindPreviousTestIdAsync(test);

            if (previousId.HasValue)
            {
                await DbContext.BackflowTests
                    .IgnoreQueryFilters()
                    .Where(t => t.Id == previousId.Value)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsCurrent, true));
            }
        }
        else
        {
            await DbContext.SaveChangesAsync();

            await ReassignIsCurrentForDeviceAsync(test, cancellationToken);
        }

        return test;
    }

    private async Task ReassignIsCurrentForDeviceAsync(BackflowTest fromTest, CancellationToken cancellationToken)
    {
        if (fromTest.SiteId == null || string.IsNullOrWhiteSpace(fromTest.SerialNumber))
        {
            return;
        }

        var serialLower = fromTest.SerialNumber.Trim().ToLower();

        var matchingTests = await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => t.SiteId == fromTest.SiteId
                     && t.Id != fromTest.Id
                     && t.DeletedTime == null
                     && !t.Rejected
                     && t.SerialNumber != null
                     && t.SerialNumber.Trim().ToLower() == serialLower)
            .Select(t => new { t.Id, t.TestDate, t.IsCurrent })
            .ToListAsync(cancellationToken);

        if (matchingTests.Count == 0)
        {
            var normalizedSerial = NormalizeSerialNumber(fromTest.SerialNumber);

            var siteCandidates = await DbContext.BackflowTests
                .IgnoreQueryFilters()
                .Where(t => t.SiteId == fromTest.SiteId && t.Id != fromTest.Id && t.DeletedTime == null && !t.Rejected && t.SerialNumber != null)
                .Select(t => new { t.Id, t.SerialNumber, t.TestDate, t.IsCurrent })
                .ToListAsync(cancellationToken);

            matchingTests = [.. siteCandidates
                .Where(t => !string.IsNullOrWhiteSpace(t.SerialNumber) && NormalizeSerialNumber(t.SerialNumber) == normalizedSerial)
                .Select(t => new { t.Id, t.TestDate, t.IsCurrent })];
        }

        if (matchingTests.Count == 0)
        {
            return;
        }

        var latestId = matchingTests.OrderByDescending(t => t.TestDate).First().Id;

        var idsToDeactivate = matchingTests
            .Where(t => t.Id != latestId && t.IsCurrent)
            .Select(t => t.Id)
            .ToList();

        if (idsToDeactivate.Count > 0)
        {
            await DbContext.BackflowTests.IgnoreQueryFilters()
                .Where(t => idsToDeactivate.Contains(t.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsCurrent, false));
        }

        var latest = matchingTests.First(t => t.Id == latestId);

        if (!latest.IsCurrent)
        {
            await DbContext.BackflowTests.IgnoreQueryFilters()
                .Where(t => t.Id == latestId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsCurrent, true));
        }
    }

    private static string NormalizeSerialNumber(string serial)
    {
        return string.Concat(serial.Where(char.IsDigit)).TrimStart('0');
    }

    private async Task<int?> FindPreviousTestIdAsync(BackflowTest fromTest)
    {
        if (string.IsNullOrWhiteSpace(fromTest.SerialNumber))
        {
            return null;
        }

        var siteId = fromTest.SiteId;
        var createdTimeCutoff = fromTest.CreatedTime;
        var streetLower = fromTest.PropertyStreetNumber?.Trim().ToLower();

        var query = DbContext.BackflowTests
            .Where(t =>
                t.SerialNumber == fromTest.SerialNumber &&
                t.Id != fromTest.Id &&
                t.DeletedTime == null &&
                !t.Rejected &&
                t.CreatedTime < createdTimeCutoff &&
                (t.SiteId == siteId ||
                 (streetLower != null && t.PropertyStreetNumber != null &&
                  t.PropertyStreetNumber.Trim().ToLower() == streetLower)));

        if (!string.IsNullOrWhiteSpace(fromTest.Manufacturer))
        {
            query = query.Where(t => t.Manufacturer == fromTest.Manufacturer);
        }

        return await query
            .OrderByDescending(t => t.SiteId == siteId ? 1 : 0)
            .ThenByDescending(t => t.CreatedTime)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();
    }
}
