using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowTestReportRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider) : Repository<BackflowTest>(dbContextSelector), IBackflowTestReportRepository
{
    private readonly TenantDbContext _tenantContext = dbContextSelector.Current;
    private readonly ITenantProvidersService _tenantProvider = tenantProvider;

    public async Task<BackflowTestReportDto> GetTestReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        // End-exclusive range so tests on the To Date (after midnight) are included.
        var start = fromDate.Date;
        var endExclusive = toDate.Date.AddDays(1);

        // Only active, in-area sites count toward the report
        var query = Entity
            .Where(t => t.TestDate >= start && t.TestDate < endExclusive
                && t.Site != null && t.Site.Active && !t.Site.OutOfArea);

        var totalCount = await query.CountAsync(cancellationToken);
        var wsId = _tenantProvider.WaterSupplierId;

        // The Rain/Freeze and OSSF stat categories are only shown when the supplier enables them.
        var settings = await _tenantContext.BackflowSettings
            .Select(s => new { s.ShowRainSensor, s.ShowOSSF })
            .FirstOrDefaultAsync(cancellationToken);

        return new BackflowTestReportDto
        {
            TotalCount = totalCount,
            Periods = await BuildPeriodsAsync(query, fromDate, toDate, totalCount, cancellationToken),
            Stats = await BuildStatsAsync(query, totalCount, settings?.ShowRainSensor ?? false, settings?.ShowOSSF ?? false, cancellationToken),
            SubAccounts = await BuildSubAccountsAsync(start, endExclusive, wsId, cancellationToken)
        };
    }

    public async Task<DateTime?> GetEarliestTestDateAsync(CancellationToken cancellationToken)
    {
        return await Entity
            .Where(t => t.TestDate != null)
            .OrderBy(t => t.TestDate)
            .Select(t => t.TestDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task<List<BackflowReportPeriodDto>> BuildPeriodsAsync(IQueryable<BackflowTest> query, DateTime fromDate, DateTime toDate, int totalCount, CancellationToken cancellationToken)
    {
        var isYearView = fromDate.Year != toDate.Year || (toDate - fromDate).TotalDays > 365;

        if (isYearView)
        {
            var byYear = await query
                .Where(t => t.TestDate.HasValue)
                .GroupBy(t => t.TestDate!.Value.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var countByYear = byYear.ToDictionary(x => x.Year, x => x.Count);

            return Enumerable.Range(fromDate.Year, toDate.Year - fromDate.Year + 1)
                .OrderByDescending(year => year)
                .Select(year =>
                {
                    var count = countByYear.GetValueOrDefault(year, 0);
                    return new BackflowReportPeriodDto { Label = year.ToString(), Count = count, Percentage = CalculatePercentage(count, totalCount), Year = year };
                })
                .ToList();
        }
        else
        {
            var year = fromDate.Year;

            var byMonth = await query
                .Where(t => t.TestDate.HasValue)
                .GroupBy(t => t.TestDate!.Value.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var countByMonth = byMonth.ToDictionary(x => x.Month, x => x.Count);

            var startMonth = fromDate.Month;
            var endMonth = toDate.Month;

            return Enumerable.Range(startMonth, endMonth - startMonth + 1)
                .Select(month =>
                {
                    var count = countByMonth.GetValueOrDefault(month, 0);
                    return new BackflowReportPeriodDto
                    {
                        Label = new DateTime(year, month, 1).ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                        Count = count,
                        Percentage = CalculatePercentage(count, totalCount),
                        Year = year,
                        Month = month
                    };
                })
                .ToList();
        }
    }

    private async Task<List<BackflowReportStatCategoryDto>> BuildStatsAsync(IQueryable<BackflowTest> query, int totalCount, bool showRainSensor, bool showOSSF, CancellationToken cancellationToken)
    {
        // The fixed single-value counts (property type, test result, reason for test) are computed in one
        // round trip via conditional aggregation, instead of a CountAsync per statistic.
        var fixedCounts = await GetFixedCountsAsync(query, cancellationToken);

        // Category order: Added By, Property Type, Test Result, Reason for Test, Hazard Type,
        // Assembly Type, then the supplier-gated Rain/Freeze and OSSF categories.
        var stats = new List<BackflowReportStatCategoryDto>
        {
            await BuildAddedByStatsAsync(query, totalCount, cancellationToken),
            BuildPropertyTypeStats(fixedCounts, totalCount),
            BuildTestResultStats(fixedCounts, totalCount),
            BuildReasonForTestStats(fixedCounts, totalCount),
            await BuildHazardTypeStatsAsync(query, totalCount, cancellationToken),
            await BuildAssemblyTypeStatsAsync(query, totalCount, cancellationToken)
        };

        // Rain/Freeze Sensor — only when the supplier enables it.
        if (showRainSensor)
        {
            stats.Add(await BuildRainFreezeStatsAsync(query, totalCount, cancellationToken));
        }

        // On-site Sewage Facility — only when the supplier enables it.
        if (showOSSF)
        {
            stats.Add(await BuildOssfStatsAsync(query, totalCount, cancellationToken));
        }

        return stats;
    }

    // Added By: classify each test by the professional who created it (BPAT / CSI).
    private async Task<BackflowReportStatCategoryDto> BuildAddedByStatsAsync(IQueryable<BackflowTest> query, int totalCount, CancellationToken cancellationToken)
    {
        var bpat = await query.CountAsync(t => t.CreatedById != null
            && _tenantContext.ProfessionalUsers.Any(pu => pu.UserId == t.CreatedById && pu.IsBackflowTester), cancellationToken);

        var csi = await query.CountAsync(t => t.CreatedById != null
            && !_tenantContext.ProfessionalUsers.Any(pu => pu.UserId == t.CreatedById && pu.IsBackflowTester)
            && _tenantContext.ProfessionalUsers.Any(pu => pu.UserId == t.CreatedById && pu.IsCsiInspector), cancellationToken);

        return new BackflowReportStatCategoryDto
        {
            Title = "Added By",
            Items =
            [
                CreateStatItem("BPAT", bpat, totalCount),
                CreateStatItem("CSI", csi, totalCount)
            ]
        };
    }

    // One round trip for every fixed single-value count: conditional aggregation over the same filtered
    // query (translated to COUNT(CASE WHEN ...) columns), rather than a CountAsync per statistic.
    private static async Task<FixedStatCounts> GetFixedCountsAsync(IQueryable<BackflowTest> query, CancellationToken cancellationToken)
    {
        // Property Type (0 = Residential, 1 = Commercial).
        var counts = await query
            .GroupBy(_ => 1)
            .Select(g => new FixedStatCounts
            {
                Residential = g.Count(t => t.PropertyType == (int)PropertyType.Residential),
                Commercial = g.Count(t => t.PropertyType == (int)PropertyType.Commercial),
                Passed = g.Count(t => t.TestResult == BackflowTestResult.Pass),
                PassedAfterRepairs = g.Count(t => t.TestResult == BackflowTestResult.PassAfterRepairs),
                Failed = g.Count(t => t.TestResult == BackflowTestResult.Fail),
                AnnualTest = g.Count(t => t.ReasonForTest == BackflowReasonForTest.AnnualTest),
                NewInstallation = g.Count(t => t.ReasonForTest == BackflowReasonForTest.NewInstallation),
                ExistingInstallation = g.Count(t => t.ReasonForTest == BackflowReasonForTest.ExistingInstallation),
                Replacement = g.Count(t => t.ReasonForTest == BackflowReasonForTest.Replacement),
                Repair = g.Count(t => t.ReasonForTest == BackflowReasonForTest.Repair),
                AnnualTestAfterRepairs = g.Count(t => t.ReasonForTest == BackflowReasonForTest.AnnualTestAfterRepairs)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // No matching tests yields no group; every count is then zero (matches per-statistic CountAsync).
        return counts ?? new FixedStatCounts();
    }

    private static BackflowReportStatCategoryDto BuildPropertyTypeStats(FixedStatCounts counts, int totalCount)
    {
        return new BackflowReportStatCategoryDto
        {
            Title = "Property Type",
            Items =
            [
                CreateStatItem("Residential", counts.Residential, totalCount),
                CreateStatItem("Commercial", counts.Commercial, totalCount)
            ]
        };
    }

    private static BackflowReportStatCategoryDto BuildTestResultStats(FixedStatCounts counts, int totalCount)
    {
        return new BackflowReportStatCategoryDto
        {
            Title = "Test Result",
            Items =
            [
                CreateStatItem("Passed", counts.Passed, totalCount),
                CreateStatItem("Passed After Repairs", counts.PassedAfterRepairs, totalCount),
                CreateStatItem("Failed", counts.Failed, totalCount)
            ]
        };
    }

    private static BackflowReportStatCategoryDto BuildReasonForTestStats(FixedStatCounts counts, int totalCount)
    {
        return new BackflowReportStatCategoryDto
        {
            Title = "Reason for Test",
            Items =
            [
                CreateStatItem("Annual Test", counts.AnnualTest, totalCount),
                CreateStatItem("New Installation", counts.NewInstallation, totalCount),
                CreateStatItem("Existing Installation", counts.ExistingInstallation, totalCount),
                CreateStatItem("Replacement Assembly", counts.Replacement, totalCount),
                CreateStatItem("Repair", counts.Repair, totalCount),
                CreateStatItem("Annual Test After Repairs", counts.AnnualTestAfterRepairs, totalCount)
            ]
        };
    }

    private static async Task<BackflowReportStatCategoryDto> BuildHazardTypeStatsAsync(IQueryable<BackflowTest> query, int totalCount, CancellationToken cancellationToken)
    {
        var hazardTypes = await query
            .GroupBy(t => t.HazardType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        return new BackflowReportStatCategoryDto
        {
            Title = "Hazard Type",
            Items = [..hazardTypes.Select(x => CreateStatItem(string.IsNullOrWhiteSpace(x.Type) ? "Unknown" : x.Type, x.Count, totalCount))]
        };
    }

    private static async Task<BackflowReportStatCategoryDto> BuildAssemblyTypeStatsAsync(IQueryable<BackflowTest> query, int totalCount, CancellationToken cancellationToken)
    {
        // Assembly Type (DeviceType)
        var assemblyTypes = await query
            .GroupBy(t => t.DeviceType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        return new BackflowReportStatCategoryDto
        {
            Title = "Assembly Type",
            Items = [..assemblyTypes.Select(x => CreateStatItem(string.IsNullOrWhiteSpace(x.Type) ? "Unknown" : x.Type, x.Count, totalCount))]
        };
    }

    private static async Task<BackflowReportStatCategoryDto> BuildRainFreezeStatsAsync(IQueryable<BackflowTest> query, int totalCount, CancellationToken cancellationToken)
    {
        // Both counts in one round trip.
        var counts = await query
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Installed = g.Count(t => t.RainFreezeSensorInstalled),
                Working = g.Count(t => t.RainFreezeSensorWorkingProperly)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new BackflowReportStatCategoryDto
        {
            Title = "Rain / Freeze Sensor",
            Items =
            [
                CreateStatItem("Installed", counts?.Installed ?? 0, totalCount),
                CreateStatItem("Working Properly", counts?.Working ?? 0, totalCount)
            ]
        };
    }

    private static async Task<BackflowReportStatCategoryDto> BuildOssfStatsAsync(IQueryable<BackflowTest> query, int totalCount, CancellationToken cancellationToken)
    {
        var ossf = await query.CountAsync(t => t.Ossf, cancellationToken);

        return new BackflowReportStatCategoryDto
        {
            Title = "On-site Sewage Facility",
            Items =
            [
                CreateStatItem("On-Site Sewage Facility", ossf, totalCount)
            ]
        };
    }

    private static BackflowReportStatItemDto CreateStatItem(string label, int count, int totalCount)
    {
        return new BackflowReportStatItemDto
        {
            Label = label,
            Count = count,
            Percentage = CalculatePercentage(count, totalCount)
        };
    }

    // Holds the fixed single-value stat counts fetched in one conditional-aggregation query.
    private sealed class FixedStatCounts
    {
        public int Residential { get; init; }
        public int Commercial { get; init; }
        public int Passed { get; init; }
        public int PassedAfterRepairs { get; init; }
        public int Failed { get; init; }
        public int AnnualTest { get; init; }
        public int NewInstallation { get; init; }
        public int ExistingInstallation { get; init; }
        public int Replacement { get; init; }
        public int Repair { get; init; }
        public int AnnualTestAfterRepairs { get; init; }
    }

    private async Task<List<BackflowSubAccountReportItemDto>> BuildSubAccountsAsync(DateTime start, DateTime endExclusive, int wsId, CancellationToken cancellationToken)
    {
        var childWaterSuppliers = await _tenantContext.WaterSuppliers
            .Where(ws => ws.ParentId == wsId)
            .Select(ws => new { ws.Id, ws.Name })
            .ToListAsync(cancellationToken);

        if (childWaterSuppliers.Count == 0)
        {
            return [];
        }

        var childIds = childWaterSuppliers.Select(ws => ws.Id).ToList();

        // Same active/in-area site rule as the main report, so the breakdown counts the same reportable
        // tests (IgnoreQueryFilters only lifts the tenant filter so child-supplier tests are visible).
        var subQuery = _tenantContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => childIds.Contains(t.WaterSupplierId)
                && t.TestDate >= start && t.TestDate < endExclusive
                && t.Site != null && t.Site.Active && !t.Site.OutOfArea);

        var subCounts = await subQuery
            .GroupBy(t => t.WaterSupplierId)
            .Select(g => new { WaterSupplierId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        // The main total is own-supplier tests only (V2 tenant-scoped pattern), so each child's
        // percentage is of the sub-account subtotal to keep the breakdown internally consistent.
        var subTotal = subCounts.Sum(x => x.Count);

        return [..childWaterSuppliers.Select(ws =>
        {
            var count = subCounts.FirstOrDefault(x => x.WaterSupplierId == ws.Id)?.Count ?? 0;
            return new BackflowSubAccountReportItemDto { Name = ws.Name, Count = count, Percentage = CalculatePercentage(count, subTotal) };
        })];
    }

    private static double CalculatePercentage(int count, int total)
    {
        return total > 0 ? Math.Round((double)count / total * 100) : 0;
    }
}
