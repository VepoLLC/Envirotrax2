using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class WaterSupplierDashboardRepository : IWaterSupplierDashboardRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantProvidersService _tenantProvider;

    public WaterSupplierDashboardRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
    {
        _context = dbContextSelector.Current;
        _tenantProvider = tenantProvider;
    }

    public async Task<WaterSupplierDashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var now = DateTime.UtcNow;
        var in30Days = now.AddDays(30);

        var professionalIds = _context.ProfessionalWaterSuppliers
            .IgnoreQueryFilters()
            .Where(pws => pws.WaterSupplierId == wsId)
            .Select(pws => pws.ProfessionalId);

        var users = _context.ProfessionalUsers.IgnoreQueryFilters();
        var licenses = _context.ProfessionalUserLicenses.IgnoreQueryFilters();
        var insurances = _context.ProfessionalInsurances.IgnoreQueryFilters();
        var gauges = _context.BackflowGauges.IgnoreQueryFilters();

        return new WaterSupplierDashboardStatsDto
        {
            WiseGuyCount       = await users.CountAsync(pu => professionalIds.Contains(pu.ProfessionalId) && pu.IsWiseGuy, cancellationToken),
            CsiInspectorCount  = await users.CountAsync(pu => professionalIds.Contains(pu.ProfessionalId) && pu.IsCsiInspector, cancellationToken),
            BpatCount = await users.CountAsync(pu => professionalIds.Contains(pu.ProfessionalId) && pu.IsBackflowTester, cancellationToken),
            FogTransporterCount = await users.CountAsync(pu => professionalIds.Contains(pu.ProfessionalId) && pu.IsFogTransporter, cancellationToken),
            FogInspectorCount = await users.CountAsync(pu => professionalIds.Contains(pu.ProfessionalId) && pu.IsFogInspector, cancellationToken),

            UnverifiedLicenseCount = await licenses.CountAsync(l => professionalIds.Contains(l.ProfessionalId) && l.ExpirationDate == null, cancellationToken),
            ExpiredLicenseCount = await licenses.CountAsync(l => professionalIds.Contains(l.ProfessionalId) && l.ExpirationDate < now, cancellationToken),
            ExpiringLicenseCount = await licenses.CountAsync(l => professionalIds.Contains(l.ProfessionalId) && l.ExpirationDate >= now && l.ExpirationDate < in30Days, cancellationToken),

            InsurancePolicyCount = await insurances.CountAsync(i => professionalIds.Contains(i.ProfessionalId) && i.ExpirationDate == null, cancellationToken),
            TestGaugeCount = await gauges.CountAsync(g => professionalIds.Contains(g.ProfessionalId) && g.LastCalibrationDate == null, cancellationToken),
            TransporterRegistrationCount = await licenses.CountAsync(l => professionalIds.Contains(l.ProfessionalId) && l.LicenseTypeId == 9 && l.ExpirationDate == null, cancellationToken)
        };
    }

    public async Task<CsiSubmissionStatsDto> GetCsiSubmissionStatsAsync(CancellationToken cancellationToken)
    {
        var wsId = _tenantProvider.WaterSupplierId;
        var start = DateTime.UtcNow.Date.AddDays(-9);
        var end = DateTime.UtcNow.Date.AddDays(1);

        var allDates = Enumerable.Range(0, 10)
            .Select(i => DateOnly.FromDateTime(start.AddDays(i)))
            .ToList();

        var rawMain = await _context.CsiInspections
            .IgnoreQueryFilters()
            .Where(c => c.WaterSupplierId == wsId && c.CreatedTime >= start && c.CreatedTime < end)
            .GroupBy(c => c.CreatedTime.Date)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Count(),
                Paid = g.Count(c => c.TransactionId != null && c.TransactionId != "")
            })
            .ToListAsync(cancellationToken);

        var dailyStats = allDates.Select(d => new CsiDailyStatsDto
        {
            Date = d,
            TotalInspections = rawMain.FirstOrDefault(r => DateOnly.FromDateTime(r.Date) == d)?.Total ?? 0,
            TotalPaidInspections = rawMain.FirstOrDefault(r => DateOnly.FromDateTime(r.Date) == d)?.Paid ?? 0
        }).ToList();

        var childWaterSuppliers = await _context.WaterSuppliers
            .IgnoreQueryFilters()
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        List<CsiSubAccountStatsDto>? subAccountStats = null;
        if (childWaterSuppliers.Count > 0)
        {
            var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
            var rawSub = await _context.CsiInspections
                .IgnoreQueryFilters()
                .Where(c => childIds.Contains(c.WaterSupplierId) && c.CreatedTime >= start && c.CreatedTime < end)
                .GroupBy(c => new { c.WaterSupplierId, c.CreatedTime.Date })
                .Select(g => new
                {
                    g.Key.WaterSupplierId,
                    g.Key.Date,
                    Total = g.Count(),
                    Paid = g.Count(c => c.TransactionId != null && c.TransactionId != "")
                })
                .ToListAsync(cancellationToken);

            subAccountStats = [..childWaterSuppliers.Select(ws => new CsiSubAccountStatsDto
            {
                WaterSupplierName = ws.Name,
                DailyStats = [..allDates.Select(d => new CsiDailyStatsDto
                {
                    Date = d,
                    TotalInspections = rawSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && DateOnly.FromDateTime(r.Date) == d)?.Total ?? 0,
                    TotalPaidInspections = rawSub.FirstOrDefault(r => r.WaterSupplierId == ws.Id && DateOnly.FromDateTime(r.Date) == d)?.Paid ?? 0
                })]
            })];
        }

        return new CsiSubmissionStatsDto { DailyStats = dailyStats, SubAccountStats = subAccountStats };
    }
}
