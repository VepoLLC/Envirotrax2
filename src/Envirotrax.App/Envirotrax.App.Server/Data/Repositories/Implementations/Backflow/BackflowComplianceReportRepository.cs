using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowComplianceReportRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper, ITenantProvidersService tenantProvider) : Repository<BackflowTest>(dbContextSelector), IBackflowComplianceReportRepository
{
    private readonly TenantDbContext _tenantContext = dbContextSelector.Current;
    private readonly ITimeZoneHelperService _timeZoneHelper = timeZoneHelper;
    private readonly ITenantProvidersService _tenantProvider = tenantProvider;

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

    public async Task<BackflowComplianceCounts> CountComplianceAsync(DateTime reportDate, CancellationToken cancellationToken)
    {
        var supplierId = _tenantProvider.WaterSupplierId;

        var supplierIds = await _tenantContext.WaterSuppliers
            .Where(ws => ws.Id == supplierId || ws.ParentId == supplierId)
            .Select(ws => ws.Id)
            .ToListAsync(cancellationToken);

        // IgnoreQueryFilters: the tenant filter would restrict to the current supplier and drop the
        // direct child-supplier tests that the roll-up must include.
        var qualifyingTests = _tenantContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(t => supplierIds.Contains(t.WaterSupplierId)
                && t.DeletedTime == null
                && t.RenewalRequired
                && t.IsCurrent
                && !t.OutOfService
                && t.TransactionId != null && t.TransactionId != "");

        var counts = await qualifyingTests
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                NonCompliant = g.Count(t => t.ExpirationDate != null && t.ExpirationDate <= reportDate)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var total = counts?.Total ?? 0;
        var nonCompliant = counts?.NonCompliant ?? 0;

        return new BackflowComplianceCounts
        {
            Total = total,
            Compliant = total - nonCompliant
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

    private static double CalculatePercentage(int count, int total)
    {
        return total > 0 ? Math.Round((double)count / total * 100) : 0;
    }
}
