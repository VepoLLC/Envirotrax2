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
}
