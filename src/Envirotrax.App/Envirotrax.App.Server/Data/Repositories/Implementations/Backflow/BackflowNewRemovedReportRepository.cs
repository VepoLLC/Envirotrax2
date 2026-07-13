using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowNewRemovedReportRepository(IDbContextSelector dbContextSelector) : Repository<BackflowTest>(dbContextSelector), IBackflowNewRemovedReportRepository
{
    public async Task<BackflowNewRemovedReportDto> GetNewRemovedAsync(CancellationToken cancellationToken)
    {
        // Last 12 months.
        // "Created" = New Installation tests grouped by CreatedTime month.
        // "Removed" = Out-of-service tests grouped by OutOfServiceDate month.
        var now = DateTime.Now;
        var currentMonth = new DateTime(now.Year, now.Month, 1);
        var startMonth = currentMonth.AddMonths(-11);
        var endExclusive = currentMonth.AddMonths(1);

        var created = await Entity
            .Where(t => t.ReasonForTest == BackflowReasonForTest.NewInstallation
                && t.CreatedTime >= startMonth && t.CreatedTime < endExclusive)
            .GroupBy(t => new { t.CreatedTime.Year, t.CreatedTime.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var removed = await Entity
            .Where(t => t.OutOfService && t.OutOfServiceDate != null
                && t.OutOfServiceDate >= startMonth && t.OutOfServiceDate < endExclusive)
            .GroupBy(t => new { t.OutOfServiceDate!.Value.Year, t.OutOfServiceDate!.Value.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var createdByKey = created.ToDictionary(x => (x.Year, x.Month), x => x.Count);
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
