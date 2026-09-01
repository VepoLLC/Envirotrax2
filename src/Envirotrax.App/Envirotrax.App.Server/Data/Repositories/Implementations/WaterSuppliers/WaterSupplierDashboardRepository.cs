using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class WaterSupplierDashboardRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider, ITimeZoneHelperService timeZoneHelper) : IWaterSupplierDashboardRepository
{
    private readonly TenantDbContext _context = dbContextSelector.Current;
    private readonly ITenantProvidersService _tenantProvider = tenantProvider;
    private readonly ITimeZoneHelperService _timeZoneHelper = timeZoneHelper;

    public async Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var now = _timeZoneHelper.GetUserLocalTime();
        var in30Days = now.AddDays(30);

        return new WaterSupplierDashboardStatsDto
        {
            PastDuePropertyLogCount = await _context.SiteLogs.CountAsync(pl => pl.ReviewDate <= now, cancellationToken),
            ExpiringPropertyLogCount = await _context.SiteLogs.CountAsync(pl => pl.ReviewDate > now && pl.ReviewDate < in30Days, cancellationToken),
            AllPropertyLogCount = await _context.SiteLogs.CountAsync(cancellationToken),

            WiseGuyCount = await _context.ProfessionalUsers.CountAsync(pu => pu.IsWiseGuy, cancellationToken),
            CsiInspectorCount = await _context.ProfessionalUsers.CountAsync(pu => pu.IsCsiInspector, cancellationToken),
            BpatCount = await _context.ProfessionalUsers.CountAsync(pu => pu.IsBackflowTester, cancellationToken),
            FogTransporterCount = await _context.ProfessionalUsers.CountAsync(pu => pu.IsFogTransporter, cancellationToken),
            FogInspectorCount = await _context.ProfessionalUsers.CountAsync(pu => pu.IsFogInspector, cancellationToken),

            UnverifiedLicenseCount = await _context.ProfessionalUserLicenses.CountAsync(l => l.ExpirationDate == null, cancellationToken),
            ExpiredLicenseCount = await _context.ProfessionalUserLicenses.CountAsync(l => l.ExpirationDate < now, cancellationToken),
            ExpiringLicenseCount = await _context.ProfessionalUserLicenses.CountAsync(l => l.ExpirationDate >= now && l.ExpirationDate < in30Days, cancellationToken),

            InsurancePolicyCount = await _context.ProfessionalInsurances.CountAsync(i => i.ExpirationDate == null, cancellationToken),
            TestGaugeCount = await _context.BackflowGauges.CountAsync(g => g.LastCalibrationDate == null, cancellationToken),
            TransporterRegistrationCount = await _context.ProfessionalUserLicenses.CountAsync(l => l.LicenseTypeId == 9 && l.ExpirationDate == null, cancellationToken)
        };
    }

    public async Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var userTz = _timeZoneHelper.GetUserTimeZone();
        var localToday = _timeZoneHelper.GetUserLocalTime().Date;
        var localStart = localToday.AddDays(-9);

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), userTz);

        var allDates = Enumerable.Range(0, 10)
            .Select(i => DateOnly.FromDateTime(localStart.AddDays(i)))
            .ToList();

        var rawMain = await _context.CsiInspections
            .Where(c => c.WaterSupplierId == wsId && c.CreatedTime >= utcStart && c.CreatedTime < utcEnd)
            .Select(c => new { c.CreatedTime, c.TransactionId })
            .ToListAsync(cancellationToken);

        var groupedMain = rawMain
            .GroupBy(c => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(c.CreatedTime, userTz)))
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Paid = g.Count(c => !string.IsNullOrEmpty(c.TransactionId))
            })
            .ToList();

        var dailyStats = allDates.Select(d => new CsiDailyStatsDto
        {
            Date = d,
            IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            TotalInspections = groupedMain.FirstOrDefault(r => r.Date == d)?.Total ?? 0,
            TotalPaidInspections = groupedMain.FirstOrDefault(r => r.Date == d)?.Paid ?? 0
        }).ToList();

        var childWaterSuppliers = await _context.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        List<CsiSubAccountStatsDto>? subAccountStats = null;
        if (childWaterSuppliers.Count > 0)
        {
            var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
            var rawSub = await _context.CsiInspections
                .IgnoreQueryFilters()
                .Where(c => childIds.Contains(c.WaterSupplierId) && c.CreatedTime >= utcStart && c.CreatedTime < utcEnd)
                .Select(c => new { c.WaterSupplierId, c.CreatedTime, c.TransactionId })
                .ToListAsync(cancellationToken);

            var groupedSub = rawSub
                .GroupBy(c => new { c.WaterSupplierId, Date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(c.CreatedTime, userTz)) })
                .Select(g => new
                {
                    g.Key.WaterSupplierId,
                    g.Key.Date,
                    Total = g.Count(),
                    Paid = g.Count(c => !string.IsNullOrEmpty(c.TransactionId))
                })
                .ToList();

            subAccountStats = [..childWaterSuppliers.Select(ws => new CsiSubAccountStatsDto
            {
                WaterSupplierId = ws.Id,
                WaterSupplierName = ws.Name,
                DailyStats = [..allDates.Select(d => new CsiDailyStatsDto
                {
                    Date = d,
                    IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                    TotalInspections = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Total ?? 0,
                    TotalPaidInspections = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Paid ?? 0
                })]
            })];
        }

        return new CsiSubmissionStatsDto { DailyStats = dailyStats, SubAccountStats = subAccountStats };
    }

    public async Task<BackflowSubmissionStatsDto> GetBackflowSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var userTz = _timeZoneHelper.GetUserTimeZone();
        var localToday = _timeZoneHelper.GetUserLocalTime().Date;
        var localStart = localToday.AddDays(-9);

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), userTz);

        var allDates = Enumerable.Range(0, 10)
            .Select(i => DateOnly.FromDateTime(localStart.AddDays(i)))
            .ToList();

        var rawMain = await _context.BackflowTests
            .Where(b => b.WaterSupplierId == wsId && b.TestDate >= utcStart && b.TestDate < utcEnd)
            .Select(b => new { b.TestDate, b.TransactionId })
            .ToListAsync(cancellationToken);

        var groupedMain = rawMain
            .GroupBy(b => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(b.TestDate!.Value, userTz)))
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Paid = g.Count(b => !string.IsNullOrEmpty(b.TransactionId))
            })
            .ToList();

        var dailyStats = allDates.Select(d => new BackflowDailyStatsDto
        {
            Date = d,
            IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            TotalTests = groupedMain.FirstOrDefault(r => r.Date == d)?.Total ?? 0,
            TotalPaidTests = groupedMain.FirstOrDefault(r => r.Date == d)?.Paid ?? 0
        }).ToList();

        var childWaterSuppliers = await _context.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        List<BackflowSubAccountStatsDto>? subAccountStats = null;
        if (childWaterSuppliers.Count > 0)
        {
            var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
            var rawSub = await _context.BackflowTests
                .IgnoreQueryFilters()
                .Where(b => childIds.Contains(b.WaterSupplierId) && b.TestDate >= utcStart && b.TestDate < utcEnd)
                .Select(b => new { b.WaterSupplierId, b.TestDate, b.TransactionId })
                .ToListAsync(cancellationToken);

            var groupedSub = rawSub
                .GroupBy(b => new { b.WaterSupplierId, Date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(b.TestDate!.Value, userTz)) })
                .Select(g => new
                {
                    g.Key.WaterSupplierId,
                    g.Key.Date,
                    Total = g.Count(),
                    Paid = g.Count(b => !string.IsNullOrEmpty(b.TransactionId))
                })
                .ToList();

            subAccountStats = [..childWaterSuppliers.Select(ws => new BackflowSubAccountStatsDto
            {
                WaterSupplierId = ws.Id,
                WaterSupplierName = ws.Name,
                DailyStats = [..allDates.Select(d => new BackflowDailyStatsDto
                {
                    Date = d,
                    IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                    TotalTests = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Total ?? 0,
                    TotalPaidTests = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Paid ?? 0
                })]
            })];
        }

        return new BackflowSubmissionStatsDto { DailyStats = dailyStats, SubAccountStats = subAccountStats };
    }

    public async Task<FogInspectionSubmissionStatsDto> GetFogInspectionSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var userTz = _timeZoneHelper.GetUserTimeZone();
        var localToday = _timeZoneHelper.GetUserLocalTime().Date;
        var localStart = localToday.AddDays(-9);

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), userTz);

        var allDates = Enumerable.Range(0, 10)
            .Select(i => DateOnly.FromDateTime(localStart.AddDays(i)))
            .ToList();

        var rawMain = await _context.FogInspections
            .Where(f => f.WaterSupplierId == wsId && f.CreatedTime >= utcStart && f.CreatedTime < utcEnd)
            .Select(f => new { f.CreatedTime, f.TransactionId })
            .ToListAsync(cancellationToken);

        var groupedMain = rawMain
            .GroupBy(f => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(f.CreatedTime, userTz)))
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Paid = g.Count(f => !string.IsNullOrEmpty(f.TransactionId))
            })
            .ToList();

        var dailyStats = allDates.Select(d => new FogInspectionDailyStatsDto
        {
            Date = d,
            IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            TotalInspections = groupedMain.FirstOrDefault(r => r.Date == d)?.Total ?? 0,
            TotalPaidInspections = groupedMain.FirstOrDefault(r => r.Date == d)?.Paid ?? 0
        }).ToList();

        var childWaterSuppliers = await _context.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        List<FogInspectionSubAccountStatsDto>? subAccountStats = null;
        if (childWaterSuppliers.Count > 0)
        {
            var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
            var rawSub = await _context.FogInspections
                .IgnoreQueryFilters()
                .Where(f => childIds.Contains(f.WaterSupplierId) && f.CreatedTime >= utcStart && f.CreatedTime < utcEnd)
                .Select(f => new { f.WaterSupplierId, f.CreatedTime, f.TransactionId })
                .ToListAsync(cancellationToken);

            var groupedSub = rawSub
                .GroupBy(f => new { f.WaterSupplierId, Date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(f.CreatedTime, userTz)) })
                .Select(g => new
                {
                    g.Key.WaterSupplierId,
                    g.Key.Date,
                    Total = g.Count(),
                    Paid = g.Count(f => !string.IsNullOrEmpty(f.TransactionId))
                })
                .ToList();

            subAccountStats = [..childWaterSuppliers.Select(ws => new FogInspectionSubAccountStatsDto
            {
                WaterSupplierId = ws.Id,
                WaterSupplierName = ws.Name,
                DailyStats = [..allDates.Select(d => new FogInspectionDailyStatsDto
                {
                    Date = d,
                    IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                    TotalInspections = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Total ?? 0,
                    TotalPaidInspections = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Paid ?? 0
                })]
            })];
        }

        return new FogInspectionSubmissionStatsDto { DailyStats = dailyStats, SubAccountStats = subAccountStats };
    }

    public async Task<FogTripTicketSubmissionStatsDto> GetFogTripTicketSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var userTz = _timeZoneHelper.GetUserTimeZone();
        var localToday = _timeZoneHelper.GetUserLocalTime().Date;
        var localStart = localToday.AddDays(-9);

        var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, userTz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(localToday.AddDays(1), userTz);

        var allDates = Enumerable.Range(0, 10)
            .Select(i => DateOnly.FromDateTime(localStart.AddDays(i)))
            .ToList();

        var rawMain = await _context.FogTripTickets
            .Where(t => t.WaterSupplierId == wsId && t.CreatedTime >= utcStart && t.CreatedTime < utcEnd)
            .Select(t => new { t.CreatedTime, t.TransactionId })
            .ToListAsync(cancellationToken);

        var groupedMain = rawMain
            .GroupBy(t => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(t.CreatedTime, userTz)))
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Paid = g.Count(t => !string.IsNullOrEmpty(t.TransactionId))
            })
            .ToList();

        var dailyStats = allDates.Select(d => new FogTripTicketDailyStatsDto
        {
            Date = d,
            IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
            TotalTripTickets = groupedMain.FirstOrDefault(r => r.Date == d)?.Total ?? 0,
            TotalPaidTripTickets = groupedMain.FirstOrDefault(r => r.Date == d)?.Paid ?? 0
        }).ToList();

        var childWaterSuppliers = await _context.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        List<FogTripTicketSubAccountStatsDto>? subAccountStats = null;
        if (childWaterSuppliers.Count > 0)
        {
            var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
            var rawSub = await _context.FogTripTickets
                .IgnoreQueryFilters()
                .Where(t => childIds.Contains(t.WaterSupplierId) && t.CreatedTime >= utcStart && t.CreatedTime < utcEnd)
                .Select(t => new { t.WaterSupplierId, t.CreatedTime, t.TransactionId })
                .ToListAsync(cancellationToken);

            var groupedSub = rawSub
                .GroupBy(t => new { t.WaterSupplierId, Date = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(t.CreatedTime, userTz)) })
                .Select(g => new
                {
                    g.Key.WaterSupplierId,
                    g.Key.Date,
                    Total = g.Count(),
                    Paid = g.Count(t => !string.IsNullOrEmpty(t.TransactionId))
                })
                .ToList();

            subAccountStats = [..childWaterSuppliers.Select(ws => new FogTripTicketSubAccountStatsDto
            {
                WaterSupplierId = ws.Id,
                WaterSupplierName = ws.Name,
                DailyStats = [..allDates.Select(d => new FogTripTicketDailyStatsDto
                {
                    Date = d,
                    IsWeekend = d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
                    TotalTripTickets = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Total ?? 0,
                    TotalPaidTripTickets = groupedSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && r.Date == d)?.Paid ?? 0
                })]
            })];
        }

        return new FogTripTicketSubmissionStatsDto { DailyStats = dailyStats, SubAccountStats = subAccountStats };
    }
}
