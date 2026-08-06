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

public class FogTripTicketReportRepository : Repository<FogTripTicket>, IFogTripTicketReportRepository
{
    private static readonly string[] _interceptorTypes = ["Grease Trap", "Grit Trap", "Septic Tank", "Chemical Toilet", "Other"];

    private readonly TenantDbContext _context;
    private readonly ITenantProvidersService _tenantProvider;

    public FogTripTicketReportRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider): base(dbContextSelector)
    {
        _context = dbContextSelector.Current;
        _tenantProvider = tenantProvider;
    }

    public async Task<FogSystemReportDto> GetTripTicketReportAsync(FogTripTicketReportDateType dateType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var subAccounts = await GetSubAccountsAsync(cancellationToken);
        var scopeIds = GetScopeIds(subAccounts);

        var tickets = GetReportableTickets(scopeIds);
        var rows = SelectRows(tickets, dateType, fromDate, toDate);

        var totalCount = await rows.CountAsync(cancellationToken);
        var totalGallons = await rows.SumAsync(row => row.Gallons, cancellationToken);
        var totalCubicFeet = await rows.SumAsync(row => row.CubicFeet, cancellationToken);

        var stats = new List<FogReportStatCategoryDto>
        {
            await GetPropertyTypeStatsAsync(rows, totalCount, cancellationToken),
            await GetInterceptorTypeStatsAsync(rows, totalCount, cancellationToken),
            await GetDisposalSiteStatsAsync(rows, totalCount, cancellationToken)
        };

        return new FogSystemReportDto
        {
            TotalCount = totalCount,
            TotalGallons = totalGallons,
            TotalCubicFeet = totalCubicFeet,
            Periods = await GetPeriodsAsync(rows, fromDate, toDate, totalCount, cancellationToken),
            SubAccounts = await GetSubAccountStatsAsync(rows, subAccounts, totalCount, cancellationToken),
            Stats = stats
        };
    }

    public async Task<DateTime?> GetEarliestTripTicketDateAsync(CancellationToken cancellationToken)
    {
        var subAccounts = await GetSubAccountsAsync(cancellationToken);
        var scopeIds = GetScopeIds(subAccounts);

        return await GetReportableTickets(scopeIds)
            .Where(ticket => ticket.InterceptorWasteRemovedDate != null)
            .OrderBy(ticket => ticket.InterceptorWasteRemovedDate)
            .Select(ticket => ticket.InterceptorWasteRemovedDate)
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

    private IQueryable<FogTripTicket> GetReportableTickets(List<int> scopeIds)
    {
        return Entity
            .IgnoreQueryFilters()
            .Where(ticket => scopeIds.Contains(ticket.WaterSupplierId))
            .Where(ticket => ticket.Completed)
            .Where(ticket => ticket.TransactionId != null && ticket.TransactionId != "");
    }

    private static IQueryable<TripTicketRow> SelectRows(IQueryable<FogTripTicket> tickets, FogTripTicketReportDateType dateType, DateTime fromDate, DateTime toDate)
    {
        var start = fromDate.Date;
        var endExclusive = toDate.Date.AddDays(1);

        return tickets
            .Select(ticket => new TripTicketRow
            {
                WaterSupplierId = ticket.WaterSupplierId,
                Date = dateType == FogTripTicketReportDateType.ReceiverDeliveryDate
                    ? ticket.ReceiverWasteDeliveredDate
                    : ticket.InterceptorWasteRemovedDate,
                PropertyType = ticket.PropertyType,
                InterceptorType = ticket.InterceptorType,
                DisposalSiteId = ticket.ReceiverDisposalSiteId,
                DisposalSiteName = ticket.ReceiverDisposalSite!.Name,
                Gallons = ticket.InterceptorWasteRemovedAmountGallons,
                CubicFeet = ticket.InterceptorWasteRemovedAmountCubicFeet
            })
            .Where(row => row.Date >= start && row.Date < endExclusive);
    }

    private static async Task<List<FogReportPeriodDto>> GetPeriodsAsync(IQueryable<TripTicketRow> rows, DateTime fromDate, DateTime toDate, int totalCount, CancellationToken cancellationToken)
    {
        var periods = new List<FogReportPeriodDto>();

        if (fromDate.Year != toDate.Year)
        {
            var totalsByYear = await GetTotalsByAsync(rows, row => row.Date!.Value.Year, cancellationToken);

            for (var year = toDate.Year; year >= fromDate.Year; year--)
            {
                var yearTotals = GetTotals(totalsByYear, year);

                periods.Add(CreatePeriod(year.ToString(), yearTotals, totalCount, year, null));
            }

            return periods;
        }

        var totalsByMonth = await GetTotalsByAsync(rows, row => row.Date!.Value.Month, cancellationToken);

        for (var month = fromDate.Month; month <= toDate.Month; month++)
        {
            var monthTotals = GetTotals(totalsByMonth, month);
            var label = new DateTime(fromDate.Year, month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture);

            periods.Add(CreatePeriod(label, monthTotals, totalCount, fromDate.Year, month));
        }

        return periods;
    }

    private static async Task<List<FogSubAccountReportItemDto>> GetSubAccountStatsAsync(IQueryable<TripTicketRow> rows, List<FogReportSubAccount> subAccounts, int totalCount, CancellationToken cancellationToken)
    {
        var items = new List<FogSubAccountReportItemDto>();

        if (subAccounts.Count == 0)
        {
            return items;
        }

        var totalsByWaterSupplier = await GetTotalsByAsync(rows, row => row.WaterSupplierId, cancellationToken);

        foreach (var subAccount in subAccounts)
        {
            var totals = GetTotals(totalsByWaterSupplier, subAccount.Id);

            items.Add(new FogSubAccountReportItemDto
            {
                Name = subAccount.Name,
                Count = totals.Count,
                Percentage = CalculatePercentage(totals.Count, totalCount),
                Gallons = totals.Gallons,
                CubicFeet = totals.CubicFeet
            });
        }

        return items;
    }

    private static async Task<FogReportStatCategoryDto> GetPropertyTypeStatsAsync(IQueryable<TripTicketRow> rows, int totalCount, CancellationToken cancellationToken)
    {
        var totalsByPropertyType = await GetTotalsByAsync(rows, row => row.PropertyType, cancellationToken);

        var residentialTotals = GetTotals(totalsByPropertyType, PropertyType.Residential);
        var commercialTotals = GetTotals(totalsByPropertyType, PropertyType.Commercial);

        return new FogReportStatCategoryDto
        {
            Title = "Property Type",
            Items = new List<FogReportStatItemDto>
            {
                CreateStatItem("Residential", residentialTotals, totalCount),
                CreateStatItem("Commercial", commercialTotals, totalCount)
            }
        };
    }

    private static async Task<FogReportStatCategoryDto> GetInterceptorTypeStatsAsync(IQueryable<TripTicketRow> rows, int totalCount, CancellationToken cancellationToken)
    {
        var rowsWithInterceptorType = rows.Where(row => row.InterceptorType != null);
        var totalsByInterceptorType = await GetTotalsByAsync(rowsWithInterceptorType, row => row.InterceptorType!, cancellationToken);

        var items = new List<FogReportStatItemDto>();

        foreach (var interceptorType in _interceptorTypes)
        {
            var totals = GetTotals(totalsByInterceptorType, interceptorType);

            items.Add(CreateStatItem(interceptorType, totals, totalCount));
        }

        return new FogReportStatCategoryDto
        {
            Title = "Interceptor Type",
            Items = items
        };
    }

    private static async Task<FogReportStatCategoryDto> GetDisposalSiteStatsAsync(IQueryable<TripTicketRow> rows, int totalCount, CancellationToken cancellationToken)
    {
        var disposalSites = await rows
            .Where(row => row.DisposalSiteId != null && row.DisposalSiteName != null)
            .GroupBy(row => new { row.DisposalSiteId, row.DisposalSiteName })
            .Select(group => new
            {
                Name = group.Key.DisposalSiteName!,
                Count = group.Count(),
                Gallons = group.Sum(row => row.Gallons),
                CubicFeet = group.Sum(row => row.CubicFeet)
            })
            .OrderBy(disposalSite => disposalSite.Name)
            .ToListAsync(cancellationToken);

        var items = new List<FogReportStatItemDto>();

        foreach (var disposalSite in disposalSites)
        {
            var totals = new TripTicketTotals
            {
                Count = disposalSite.Count,
                Gallons = disposalSite.Gallons,
                CubicFeet = disposalSite.CubicFeet
            };

            items.Add(CreateStatItem(disposalSite.Name, totals, totalCount));
        }

        return new FogReportStatCategoryDto
        {
            Title = "Disposal Sites",
            Items = items
        };
    }

    private static async Task<Dictionary<TKey, TripTicketTotals>> GetTotalsByAsync<TKey>(IQueryable<TripTicketRow> rows, Expression<Func<TripTicketRow, TKey>> groupBy, CancellationToken cancellationToken)
        where TKey : notnull
    {
        var groups = await rows
            .GroupBy(groupBy)
            .Select(group => new
            {
                group.Key,
                Count = group.Count(),
                Gallons = group.Sum(row => row.Gallons),
                CubicFeet = group.Sum(row => row.CubicFeet)
            })
            .ToListAsync(cancellationToken);

        var totalsByKey = new Dictionary<TKey, TripTicketTotals>();

        foreach (var group in groups)
        {
            totalsByKey[group.Key] = new TripTicketTotals
            {
                Count = group.Count,
                Gallons = group.Gallons,
                CubicFeet = group.CubicFeet
            };
        }

        return totalsByKey;
    }

    private static TripTicketTotals GetTotals<TKey>(Dictionary<TKey, TripTicketTotals> totalsByKey, TKey key)
        where TKey : notnull
    {
        if (totalsByKey.TryGetValue(key, out var totals))
        {
            return totals;
        }

        return new TripTicketTotals();
    }

    private static FogReportPeriodDto CreatePeriod(string label, TripTicketTotals totals, int totalCount, int year, int? month)
    {
        return new FogReportPeriodDto
        {
            Label = label,
            Count = totals.Count,
            Percentage = CalculatePercentage(totals.Count, totalCount),
            Gallons = totals.Gallons,
            CubicFeet = totals.CubicFeet,
            Year = year,
            Month = month
        };
    }

    private static FogReportStatItemDto CreateStatItem(string label, TripTicketTotals totals, int totalCount)
    {
        return new FogReportStatItemDto
        {
            Label = label,
            Count = totals.Count,
            Percentage = CalculatePercentage(totals.Count, totalCount),
            Gallons = totals.Gallons,
            CubicFeet = totals.CubicFeet
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

    private sealed class TripTicketTotals
    {
        public int Count { get; set; }
        public double Gallons { get; set; }
        public double CubicFeet { get; set; }
    }

    private sealed class FogReportSubAccount
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class TripTicketRow
    {
        public int WaterSupplierId { get; set; }
        public DateTime? Date { get; set; }
        public PropertyType PropertyType { get; set; }
        public string? InterceptorType { get; set; }
        public int? DisposalSiteId { get; set; }
        public string? DisposalSiteName { get; set; }
        public double Gallons { get; set; }
        public double CubicFeet { get; set; }
    }
}
