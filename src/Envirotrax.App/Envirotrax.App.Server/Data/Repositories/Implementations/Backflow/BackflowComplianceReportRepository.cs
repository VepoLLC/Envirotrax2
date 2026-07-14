using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowComplianceReportRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper) : Repository<BackflowTest>(dbContextSelector), IBackflowComplianceReportRepository
{
    private readonly TenantDbContext _tenantContext = dbContextSelector.Current;
    private readonly ITimeZoneHelperService _timeZoneHelper = timeZoneHelper;

    private const string AllValue = "All";

    // Mirrors V1's GetHazardTypes() and the client's hazardTypeOptions — used to expand a
    // "HazardType = All" renewal requirement into one row per hazard type.
    private static readonly string[] HazardTypes =
    [
        "Agricultural/Feed Lot",
        "Domestic/Premises Isolation",
        "Fire System",
        "Gas Station/Car Wash",
        "Irrigation - Non Chemical",
        "Irrigation - Chemical Feed",
        "Laundry/Cleaners",
        "Medical/Dental/Laboratory/Mortuary",
        "Nails/Salon/Grooming",
        "Pool/Recreation/Athletics",
        "Restaurant/Vending/Grocery",
        "Fire Hydrant/Temporary Construction",
        "Fountains/Garden Ponds/Water Features",
        "Water Softener",
        "Other"
    ];

    public async Task<BackflowComplianceReportDto> GetComplianceReportAsync(bool ignoreLast30Days, CancellationToken cancellationToken)
    {
        var now = _timeZoneHelper.GetUserLocalTime();
        var cutoff = ignoreLast30Days ? now.AddDays(-30) : now;

        var requirements = await LoadRenewalRequirementsAsync(cancellationToken);
        var assemblies = await LoadCurrentComplianceAssembliesAsync(cancellationToken);

        var rows = BuildRequirementRows(requirements, assemblies, cutoff);

        return BuildComplianceReport(rows);
    }

    // The supplier's renewal requirements define the report's rows
    private async Task<List<BackflowRenewalRequirement>> LoadRenewalRequirementsAsync(CancellationToken cancellationToken)
    {
        return await _tenantContext.BackflowRenewalRequirements
            .OrderBy(r => r.PropertyType)
            .ThenBy(r => r.DeviceType)
            .ThenBy(r => r.HazardType)
            .ToListAsync(cancellationToken);
    }

    // Active assemblies in scope: the current, in-service test on an active, in-area site.
    private async Task<List<ComplianceAssembly>> LoadCurrentComplianceAssembliesAsync(CancellationToken cancellationToken)
    {
        return await Entity
            .Where(t => t.IsCurrent && !t.OutOfService
                && t.Site != null && t.Site.Active && !t.Site.OutOfArea)
            .Select(t => new ComplianceAssembly
            {
                PropertyType = t.PropertyType,
                DeviceType = t.DeviceType,
                HazardType = t.HazardType,
                HasOssf = t.Site!.HasOnSiteSewageFacility,
                HasAuxWater = t.Site!.HasAuxWaterSupply,
                ExpirationDate = t.ExpirationDate
            })
            .ToListAsync(cancellationToken);
    }

    private static List<BackflowComplianceRequirementDto> BuildRequirementRows(List<BackflowRenewalRequirement> requirements, List<ComplianceAssembly> assemblies, DateTime cutoff)
    {
        var rows = new List<BackflowComplianceRequirementDto>();

        foreach (var requirement in requirements)
        {
            // A "HazardType = All" requirement expands into one row per hazard type (V1 behaviour).
            if (string.Equals(requirement.HazardType, AllValue, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var hazardType in HazardTypes)
                {
                    rows.Add(BuildRequirementRow(requirement, hazardType, assemblies, cutoff));
                }
            }
            else
            {
                rows.Add(BuildRequirementRow(requirement, requirement.HazardType, assemblies, cutoff));
            }
        }

        return rows;
    }

    private static BackflowComplianceReportDto BuildComplianceReport(List<BackflowComplianceRequirementDto> rows)
    {
        // The grand totals (and the pie) are the sum over requirement rows, matching V1 — an assembly
        // matched by overlapping requirements is counted in each, and one matching no requirement is not shown.
        var totalActive = rows.Sum(r => r.Active);
        var compliantTotal = rows.Sum(r => r.Compliant);
        var nonCompliantTotal = totalActive - compliantTotal;

        return new BackflowComplianceReportDto
        {
            TotalActive = totalActive,
            Compliant = compliantTotal,
            NonCompliant = nonCompliantTotal,
            CompliantPercentage = CalculatePercentage(compliantTotal, totalActive),
            NonCompliantPercentage = CalculatePercentage(nonCompliantTotal, totalActive),
            Requirements = rows
        };
    }

    private static BackflowComplianceRequirementDto BuildRequirementRow(BackflowRenewalRequirement requirement, string? hazardType, List<ComplianceAssembly> assemblies, DateTime cutoff)
    {
        var matches = assemblies.Where(a =>
            a.PropertyType == (int)requirement.PropertyType
            && (string.Equals(requirement.DeviceType, AllValue, StringComparison.OrdinalIgnoreCase) || a.DeviceType == requirement.DeviceType)
            && a.HazardType == hazardType
            && (!requirement.HasSiteOssf || a.HasOssf)
            && (!requirement.AuxWaterSupply || a.HasAuxWater));

        var active = 0;
        var nonCompliant = 0;

        foreach (var assembly in matches)
        {
            active++;

            if (assembly.ExpirationDate != null && assembly.ExpirationDate <= cutoff)
            {
                nonCompliant++;
            }
        }

        var compliant = active - nonCompliant;

        return new BackflowComplianceRequirementDto
        {
            PropertyType = requirement.PropertyType == PropertyType.Commercial ? "Commercial" : "Residential",
            AssemblyType = requirement.DeviceType ?? "",
            HazardType = hazardType ?? "",
            HasSiteOssf = requirement.HasSiteOssf,
            AuxWaterSupply = requirement.AuxWaterSupply,
            RenewalYears = requirement.RenewalYears,
            Active = active,
            Compliant = compliant,
            Percentage = CalculatePercentage(compliant, active)
        };
    }

    public async Task<BackflowComplianceHistoryDto> GetComplianceHistoryAsync(CancellationToken cancellationToken)
    {
        // V2 has no monthly snapshot table, so we reconstruct each month's compliance "as of the 1st"
        // (matching V1's snapshot-on-day-1) directly from the test history. Assemblies are identified
        // by site + serial number; an assembly is active in a month if it has a test on or before that
        // month and has not been taken out of service by then; compliant if its latest test as of that
        // month has not expired. Site active/in-area state is the current state (not reconstructed).
        var tests = await LoadHistoryTestPointsAsync(cancellationToken);

        var result = new BackflowComplianceHistoryDto();
        if (tests.Count == 0)
        {
            return result;
        }

        var assemblies = BuildAssemblyHistories(tests);

        var now = _timeZoneHelper.GetUserLocalTime();
        var currentMonth = new DateTime(now.Year, now.Month, 1);
        var startMonth = GetHistoryStartMonth(tests, currentMonth);

        for (var asOf = startMonth; asOf <= currentMonth; asOf = asOf.AddMonths(1))
        {
            result.Points.Add(BuildHistoryPoint(assemblies, asOf));
        }

        return result;
    }

    private async Task<List<TestPoint>> LoadHistoryTestPointsAsync(CancellationToken cancellationToken)
    {
        return await Entity
            .Where(t => t.TestDate != null && t.Site != null && t.Site.Active && !t.Site.OutOfArea)
            .Select(t => new TestPoint
            {
                SiteId = t.SiteId,
                SerialNumber = t.SerialNumber,
                TestDate = t.TestDate!.Value,
                ExpirationDate = t.ExpirationDate,
                OutOfService = t.OutOfService,
                OutOfServiceDate = t.OutOfServiceDate
            })
            .ToListAsync(cancellationToken);
    }

    // Group tests into per-assembly histories keyed by site + normalized serial number.
    private static List<AssemblyHistory> BuildAssemblyHistories(List<TestPoint> tests)
    {
        return tests
            .GroupBy(t => $"{t.SiteId}|{(t.SerialNumber ?? "").Trim().ToUpperInvariant()}")
            .Select(g => new AssemblyHistory
            {
                Tests = g.OrderByDescending(x => x.TestDate).ToList(),
                RemovedDate = g.Where(x => x.OutOfService && x.OutOfServiceDate != null)
                    .Select(x => x.OutOfServiceDate!.Value)
                    .DefaultIfEmpty(DateTime.MaxValue)
                    .Min()
            })
            .ToList();
    }

    private static DateTime GetHistoryStartMonth(List<TestPoint> tests, DateTime currentMonth)
    {
        var earliest = tests.Min(t => t.TestDate);
        var earliestMonth = new DateTime(earliest.Year, earliest.Month, 1);
        var windowStart = currentMonth.AddMonths(-47);

        return earliestMonth > windowStart ? earliestMonth : windowStart;
    }

    // Compliance as of the 1st of a month: an assembly counts if it has a test on or before the month
    // and wasn't removed by then; it's compliant if that latest test's ExpirationDate hasn't passed.
    private static BackflowComplianceHistoryPointDto BuildHistoryPoint(List<AssemblyHistory> assemblies, DateTime asOf)
    {
        var total = 0;
        var compliant = 0;

        foreach (var assembly in assemblies)
        {
            if (assembly.RemovedDate <= asOf)
            {
                continue;
            }

            var latest = assembly.Tests.FirstOrDefault(x => x.TestDate <= asOf);
            if (latest == null)
            {
                continue;
            }

            total++;
            if (latest.ExpirationDate != null && latest.ExpirationDate.Value >= asOf)
            {
                compliant++;
            }
        }

        return new BackflowComplianceHistoryPointDto
        {
            Year = asOf.Year,
            Month = asOf.Month,
            Label = asOf.ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
            Total = total,
            Compliant = compliant,
            NonCompliant = total - compliant,
            Percentage = CalculatePercentage(compliant, total)
        };
    }

    private sealed class ComplianceAssembly
    {
        public int PropertyType { get; init; }
        public string? DeviceType { get; init; }
        public string? HazardType { get; init; }
        public bool HasOssf { get; init; }
        public bool HasAuxWater { get; init; }
        public DateTime? ExpirationDate { get; init; }
    }

    private sealed class TestPoint
    {
        public int? SiteId { get; init; }
        public string? SerialNumber { get; init; }
        public DateTime TestDate { get; init; }
        public DateTime? ExpirationDate { get; init; }
        public bool OutOfService { get; init; }
        public DateTime? OutOfServiceDate { get; init; }
    }

    private sealed class AssemblyHistory
    {
        // Tests ordered newest first, so FirstOrDefault(TestDate <= asOf) is the latest as of that month.
        public List<TestPoint> Tests { get; init; } = [];
        public DateTime RemovedDate { get; init; }
    }

    private static double CalculatePercentage(int count, int total)
    {
        return total > 0 ? Math.Round((double)count / total * 100) : 0;
    }
}
