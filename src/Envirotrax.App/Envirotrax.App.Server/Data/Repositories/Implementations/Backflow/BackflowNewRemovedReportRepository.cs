using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowNewRemovedReportRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper) : Repository<BackflowTest>(dbContextSelector), IBackflowNewRemovedReportRepository
{
    private readonly ITimeZoneHelperService _timeZoneHelper = timeZoneHelper;

    public async Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        // Last 12 months, measured in the user's time zone (not the server's local time).
        // "Created" = New Installation tests grouped by CreatedTime month.
        // "Removed" = Out-of-service tests grouped by OutOfServiceDate month.
        var userTz = _timeZoneHelper.GetUserTimeZone();
        var localNow = _timeZoneHelper.GetUserLocalTime();
        var currentMonth = new DateTime(localNow.Year, localNow.Month, 1);
        var startMonth = currentMonth.AddMonths(-11);
        var endExclusive = currentMonth.AddMonths(1);

        // CreatedTime is a UTC timestamp: query with UTC-converted bounds, then bucket each row by the
        // month it falls in once converted back to the user's local time.
        var utcStart = TimeZoneInfo.ConvertTimeToUtc(startMonth, userTz);
        var utcEnd = TimeZoneInfo.ConvertTimeToUtc(endExclusive, userTz);

        var createdTimes = await Entity
            .Where(t => t.ReasonForTest == BackflowReasonForTest.NewInstallation
                && t.CreatedTime >= utcStart && t.CreatedTime < utcEnd)
            .Select(t => t.CreatedTime)
            .ToListAsync(cancellationToken);

        var createdByKey = createdTimes
            .GroupBy(createdTime =>
            {
                var local = TimeZoneInfo.ConvertTimeFromUtc(createdTime, userTz);
                return (local.Year, local.Month);
            })
            .ToDictionary(g => g.Key, g => g.Count());

        // OutOfServiceDate is a user-entered calendar date (no time zone): compare and bucket it directly
        // against the user-local month window, with no time-zone conversion.
        var removed = await Entity
            .Where(t => t.OutOfService && t.OutOfServiceDate != null
                && t.OutOfServiceDate >= startMonth && t.OutOfServiceDate < endExclusive)
            .GroupBy(t => new { t.OutOfServiceDate!.Value.Year, t.OutOfServiceDate!.Value.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var removedByKey = removed.ToDictionary(x => (x.Year, x.Month), x => x.Count);

        // Match V1: show only months that have created or removed activity (no zero-fill of the
        // 12-month window). The dataset is the union of the two grouped results, ordered chronologically.
        var activeMonths = createdByKey.Keys
            .Union(removedByKey.Keys)
            .OrderBy(key => key.Year)
            .ThenBy(key => key.Month)
            .ToList();

        var report = new BackflowNewRemovedReportDto();
        foreach (var key in activeMonths)
        {
            var month = new DateTime(key.Year, key.Month, 1);
            report.Points.Add(new BackflowNewRemovedPointDto
            {
                Year = key.Year,
                Month = key.Month,
                Label = month.ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                Created = createdByKey.GetValueOrDefault(key, 0),
                Removed = removedByKey.GetValueOrDefault(key, 0)
            });
        }

        return report;
    }
}
