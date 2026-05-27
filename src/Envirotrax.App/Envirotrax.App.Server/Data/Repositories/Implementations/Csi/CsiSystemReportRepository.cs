using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

public class CsiSystemReportRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider) : Repository<CsiInspection>(dbContextSelector), ICsiSystemReportRepository
{
    private readonly TenantDbContext _tenantContext = dbContextSelector.Current;
    private readonly ITenantProvidersService _tenantProvider = tenantProvider;

    public async Task<CsiSystemReportDto> GetSystemReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var query = Entity.AsQueryable()
        .Where(c => c.InspectionDate >= fromDate && c.InspectionDate <= toDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var wsId = _tenantProvider.WaterSupplierId;

        return new CsiSystemReportDto
        {
            TotalCount = totalCount,
            Periods = await BuildPeriodsAsync(query, fromDate, toDate, totalCount, cancellationToken),
            Stats = await BuildStatsAsync(query, totalCount, cancellationToken),
            SubAccounts = await BuildSubAccountsAsync(fromDate, toDate, wsId, cancellationToken)
        };
    }

    private static async Task<List<CsiReportPeriodDto>> BuildPeriodsAsync(IQueryable<CsiInspection> query, DateTime fromDate, DateTime toDate, int totalCount, CancellationToken cancellationToken)
    {
        var isYearView = fromDate.Year != toDate.Year || (toDate - fromDate).TotalDays > 365;

        if (isYearView)
        {
            var byYear = await query
                .Where(c => c.InspectionDate.HasValue)
                .GroupBy(c => c.InspectionDate!.Value.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var countByYear = byYear.ToDictionary(x => x.Year, x => x.Count);

            return Enumerable.Range(fromDate.Year, toDate.Year - fromDate.Year + 1)
                .OrderByDescending(year => year)
                .Select(year =>
                {
                    var count = countByYear.GetValueOrDefault(year, 0);
                    return new CsiReportPeriodDto { Label = year.ToString(), Count = count, Percentage = Pct(count, totalCount), Year = year };
                })
                .ToList();
        }
        else
        {
            var year = fromDate.Year;

            var byMonth = await query
                .Where(c => c.InspectionDate.HasValue)
                .GroupBy(c => c.InspectionDate!.Value.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var countByMonth = byMonth.ToDictionary(x => x.Month, x => x.Count);

            var startMonth = fromDate.Month;
            var endMonth = toDate.Month;

            return Enumerable.Range(startMonth, endMonth - startMonth + 1)
                .Select(month =>
                {
                    var count = countByMonth.GetValueOrDefault(month, 0);
                    return new CsiReportPeriodDto
                    {
                        Label = new DateTime(year, month, 1).ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Count = count,
                        Percentage = Pct(count, totalCount),
                        Year = year,
                        Month = month
                    };
                })
                .ToList();
        }
    }

    private static async Task<List<CsiReportStatCategoryDto>> BuildStatsAsync(IQueryable<CsiInspection> query, int totalCount, CancellationToken cancellationToken)
    {
        var passed = await query.CountAsync(c => c.InspectionResult, cancellationToken);
        var failed = totalCount - passed;

        var residential = await query.CountAsync(c => c.PropertyType == PropertyType.Residential, cancellationToken);
        var commercial = await query.CountAsync(c => c.PropertyType == PropertyType.Commercial, cancellationToken);

        var newConstruction = await query.CountAsync(c => c.ReasonForInspection == CsiInspectionReason.NewConstruction, cancellationToken);
        var existingService = await query.CountAsync(c => c.ReasonForInspection == CsiInspectionReason.ExistingServiceContaminantHazardsSuspected, cancellationToken);
        var renovation = await query.CountAsync(c => c.ReasonForInspection == CsiInspectionReason.MajorRenovationOrExpansion, cancellationToken);

        return
        [
            new CsiReportStatCategoryDto
            {
                Title = "Inspection Result",
                Items =
                [
                    new CsiReportStatItemDto { Label = "Passed", Count = passed, Percentage = Pct(passed, totalCount) },
                    new CsiReportStatItemDto { Label = "Failed", Count = failed, Percentage = Pct(failed, totalCount) }
                ]
            },
            new CsiReportStatCategoryDto
            {
                Title = "Property Type",
                Items =
                [
                    new CsiReportStatItemDto { Label = "Residential", Count = residential, Percentage = Pct(residential, totalCount) },
                    new CsiReportStatItemDto { Label = "Commercial", Count = commercial, Percentage = Pct(commercial, totalCount) }
                ]
            },
            new CsiReportStatCategoryDto
            {
                Title = "Inspection Reason",
                Items =
                [
                    new CsiReportStatItemDto { Label = "New Construction", Count = newConstruction, Percentage = Pct(newConstruction, totalCount) },
                    new CsiReportStatItemDto { Label = "Existing Service", Count = existingService, Percentage = Pct(existingService, totalCount) },
                    new CsiReportStatItemDto { Label = "Renovation / Expansion", Count = renovation, Percentage = Pct(renovation, totalCount) }
                ]
            }
        ];
    }

    private async Task<List<CsiSubAccountReportItemDto>> BuildSubAccountsAsync(DateTime fromDate, DateTime toDate, int wsId, CancellationToken cancellationToken)
    {
        var childWaterSuppliers = await _tenantContext.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        if (childWaterSuppliers.Count == 0)
            return [];

        var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();
        var subQuery = _tenantContext.CsiInspections
            .Where(c => childIds.Contains(c.WaterSupplierId) && c.InspectionDate >= fromDate && c.InspectionDate <= toDate);

        var subCounts = await subQuery
            .GroupBy(c => c.WaterSupplierId)
            .Select(g => new { WaterSupplierId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var subTotal = subCounts.Sum(x => x.Count);

        return [..childWaterSuppliers.Select(ws =>
        {
            var count = subCounts.FirstOrDefault(x => x.WaterSupplierId == ws.Id)?.Count ?? 0;
            return new CsiSubAccountReportItemDto { Name = ws.Name, Count = count, Percentage = Pct(count, subTotal) };
        })];
    }

    private static double Pct(int count, int total)
    {
        return total > 0 ? Math.Round((double)count / total * 100) : 0;
    }
}
