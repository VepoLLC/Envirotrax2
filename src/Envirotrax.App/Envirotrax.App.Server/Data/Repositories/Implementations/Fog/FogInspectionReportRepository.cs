using System.Globalization;
using System.Linq.Expressions;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogInspectionReportRepository : Repository<FogInspection>, IFogInspectionReportRepository
{
    private static readonly (string Label, FacilityType? Type)[] _facilityTypes =
    [
        ("Grease Trap", null),
        ("Restaurant", FacilityType.Restaurant),
        ("Fast Food Establishment", FacilityType.FastFoodEstablishment),
        ("Hotel/Motel", FacilityType.HotelMotel),
        ("Car Wash", FacilityType.CarWash),
        ("School/University", FacilityType.SchoolUniversity),
        ("Grocery Store", FacilityType.GroceryStore),
        ("Convenience Store", FacilityType.ConvenienceStore),
        ("Assisted Living Facility", FacilityType.AssistedLivingFacility),
        ("Medical Facility", FacilityType.MedicalFacility),
        ("City Owned Facility", FacilityType.CityOwnedFacility),
        ("Other", FacilityType.Other)
    ];

    private static readonly string[] _interceptorTypes = ["Grease Trap", "Grit Trap", "Septic Tank", "Chemical Toilet", "Other"];

    private readonly TenantDbContext _context;
    private readonly ITenantProvidersService _tenantProvider;

    public FogInspectionReportRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider): base(dbContextSelector)
    {
        _context = dbContextSelector.Current;
        _tenantProvider = tenantProvider;
    }

    public async Task<FogSystemReportDto> GetInspectionReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var subAccounts = await GetSubAccountsAsync(cancellationToken);
        var scopeIds = GetScopeIds(subAccounts);

        var inspections = GetReportableInspections(scopeIds, fromDate, toDate);

        var totalCount = await inspections.CountAsync(cancellationToken);

        var stats = new List<FogReportStatCategoryDto>
        {
            await GetPropertyTypeStatsAsync(inspections, totalCount, cancellationToken),
            await GetFacilityTypeStatsAsync(inspections, totalCount, cancellationToken),
            await GetInterceptorTypeStatsAsync(inspections, totalCount, cancellationToken)
        };

        return new FogSystemReportDto
        {
            TotalCount = totalCount,
            Periods = await GetPeriodsAsync(inspections, fromDate, toDate, totalCount, cancellationToken),
            SubAccounts = await GetSubAccountStatsAsync(inspections, subAccounts, totalCount, cancellationToken),
            Stats = stats
        };
    }

    public async Task<DateTime?> GetEarliestInspectionDateAsync(CancellationToken cancellationToken)
    {
        var subAccounts = await GetSubAccountsAsync(cancellationToken);
        var scopeIds = GetScopeIds(subAccounts);

        return await GetScopedInspections(scopeIds)
            .Where(inspection => inspection.InspectionDate != null)
            .OrderBy(inspection => inspection.InspectionDate)
            .Select(inspection => inspection.InspectionDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<FogReportSubAccount>> GetSubAccountsAsync(CancellationToken cancellationToken)
    {
        var waterSupplierId = _tenantProvider.WaterSupplierId;

        return await _context.WaterSuppliers
            .Where(waterSupplier => waterSupplier.ParentId == waterSupplierId)
            .OrderBy(waterSupplier => waterSupplier.Name)
            .Select(waterSupplier => new FogReportSubAccount
            {
                Id = waterSupplier.Id,
                Name = waterSupplier.Name
            })
            .ToListAsync(cancellationToken);
    }

    private List<int> GetScopeIds(List<FogReportSubAccount> subAccounts)
    {
        var scopeIds = new List<int>
        {
            _tenantProvider.WaterSupplierId
        };

        foreach (var subAccount in subAccounts)
        {
            scopeIds.Add(subAccount.Id);
        }

        return scopeIds;
    }

    private IQueryable<FogInspection> GetScopedInspections(List<int> scopeIds)
    {
        return Entity
            .IgnoreQueryFilters()
            .Where(inspection => scopeIds.Contains(inspection.WaterSupplierId))
            .Where(inspection => inspection.TransactionId != null && inspection.TransactionId != "");
    }

    private IQueryable<FogInspection> GetReportableInspections(List<int> scopeIds, DateTime fromDate, DateTime toDate)
    {
        var start = fromDate.Date;
        var endExclusive = toDate.Date.AddDays(1);

        return GetScopedInspections(scopeIds)
            .Where(inspection => inspection.InspectionDate >= start && inspection.InspectionDate < endExclusive);
    }

    private static async Task<List<FogReportPeriodDto>> GetPeriodsAsync(IQueryable<FogInspection> inspections, DateTime fromDate, DateTime toDate, int totalCount, CancellationToken cancellationToken)
    {
        var periods = new List<FogReportPeriodDto>();

        if (fromDate.Year != toDate.Year)
        {
            var countsByYear = await GetCountsByAsync(inspections, inspection => inspection.InspectionDate!.Value.Year, cancellationToken);

            for (var year = toDate.Year; year >= fromDate.Year; year--)
            {
                var yearCount = GetCount(countsByYear, year);

                periods.Add(CreatePeriod(year.ToString(), yearCount, totalCount, year, null));
            }

            return periods;
        }

        var countsByMonth = await GetCountsByAsync(inspections, inspection => inspection.InspectionDate!.Value.Month, cancellationToken);

        for (var month = fromDate.Month; month <= toDate.Month; month++)
        {
            var monthCount = GetCount(countsByMonth, month);
            var label = new DateTime(fromDate.Year, month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture);

            periods.Add(CreatePeriod(label, monthCount, totalCount, fromDate.Year, month));
        }

        return periods;
    }

    private static async Task<List<FogSubAccountReportItemDto>> GetSubAccountStatsAsync(IQueryable<FogInspection> inspections, List<FogReportSubAccount> subAccounts, int totalCount, CancellationToken cancellationToken)
    {
        var items = new List<FogSubAccountReportItemDto>();

        if (subAccounts.Count == 0)
        {
            return items;
        }

        var countsByWaterSupplier = await GetCountsByAsync(inspections, inspection => inspection.WaterSupplierId, cancellationToken);

        foreach (var subAccount in subAccounts)
        {
            var count = GetCount(countsByWaterSupplier, subAccount.Id);

            items.Add(new FogSubAccountReportItemDto
            {
                Name = subAccount.Name,
                Count = count,
                Percentage = CalculatePercentage(count, totalCount)
            });
        }

        return items;
    }

    private static async Task<FogReportStatCategoryDto> GetPropertyTypeStatsAsync(IQueryable<FogInspection> inspections, int totalCount, CancellationToken cancellationToken)
    {
        var countsByPropertyType = await GetCountsByAsync(inspections, inspection => inspection.PropertyType, cancellationToken);

        var residentialCount = GetCount(countsByPropertyType, PropertyType.Residential);
        var commercialCount = GetCount(countsByPropertyType, PropertyType.Commercial);

        return new FogReportStatCategoryDto
        {
            Title = "Property Type",
            Items = new List<FogReportStatItemDto>
            {
                CreateStatItem("Residential", residentialCount, totalCount),
                CreateStatItem("Commercial", commercialCount, totalCount)
            }
        };
    }

    private static async Task<FogReportStatCategoryDto> GetFacilityTypeStatsAsync(IQueryable<FogInspection> inspections, int totalCount, CancellationToken cancellationToken)
    {
        var countsByFacilityType = await GetCountsByAsync(inspections, inspection => inspection.FacilityType, cancellationToken);

        var items = new List<FogReportStatItemDto>();

        foreach (var facilityType in _facilityTypes)
        {
            var count = 0;

            if (facilityType.Type != null)
            {
                count = GetCount(countsByFacilityType, facilityType.Type.Value);
            }

            items.Add(CreateStatItem(facilityType.Label, count, totalCount));
        }

        return new FogReportStatCategoryDto
        {
            Title = "Facility Type",
            Items = items
        };
    }

    private static async Task<FogReportStatCategoryDto> GetInterceptorTypeStatsAsync(IQueryable<FogInspection> inspections, int totalCount, CancellationToken cancellationToken)
    {
        var inspectionsWithInterceptorType = inspections.Where(inspection => inspection.InterceptorType != null);
        var countsByInterceptorType = await GetCountsByAsync(inspectionsWithInterceptorType, inspection => inspection.InterceptorType!, cancellationToken);

        var items = new List<FogReportStatItemDto>();

        foreach (var interceptorType in _interceptorTypes)
        {
            var count = GetCount(countsByInterceptorType, interceptorType);

            items.Add(CreateStatItem(interceptorType, count, totalCount));
        }

        return new FogReportStatCategoryDto
        {
            Title = "Interceptor Type",
            Items = items
        };
    }

    private static async Task<Dictionary<TKey, int>> GetCountsByAsync<TKey>(IQueryable<FogInspection> inspections, Expression<Func<FogInspection, TKey>> groupBy, CancellationToken cancellationToken)
        where TKey : notnull
    {
        var groups = await inspections
            .GroupBy(groupBy)
            .Select(group => new
            {
                group.Key,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        var countsByKey = new Dictionary<TKey, int>();

        foreach (var group in groups)
        {
            countsByKey[group.Key] = group.Count;
        }

        return countsByKey;
    }

    private static int GetCount<TKey>(Dictionary<TKey, int> countsByKey, TKey key)
        where TKey : notnull
    {
        if (countsByKey.TryGetValue(key, out var count))
        {
            return count;
        }

        return 0;
    }

    private static FogReportPeriodDto CreatePeriod(string label, int count, int totalCount, int year, int? month)
    {
        return new FogReportPeriodDto
        {
            Label = label,
            Count = count,
            Percentage = CalculatePercentage(count, totalCount),
            Year = year,
            Month = month
        };
    }

    private static FogReportStatItemDto CreateStatItem(string label, int count, int totalCount)
    {
        return new FogReportStatItemDto
        {
            Label = label,
            Count = count,
            Percentage = CalculatePercentage(count, totalCount)
        };
    }

    private static double CalculatePercentage(int count, int total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Round((double)count / total * 100);
    }

    private sealed class FogReportSubAccount
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
