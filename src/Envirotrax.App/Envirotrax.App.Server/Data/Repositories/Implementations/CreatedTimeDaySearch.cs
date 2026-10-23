using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

/// <summary>
/// Makes a date criterion on CreatedTime match whole days in the user's time zone, the same days the water supplier
/// dashboard groups submissions by, so a dashboard day's "View" list holds exactly the records counted for that day.
/// </summary>
/// <remarks>
/// CreatedTime is stored in UTC and the client sends plain dates. The filter library reads a date in the criterion's
/// ClientTimeZone, but ConvertFilterProperties resets that to UTC, so this has to run on the converted query.
/// </remarks>
public static class CreatedTimeDaySearch
{
    private const string CreatedTimeColumn = nameof(ICreateAuditableModel<>.CreatedTime);

    public static void Apply(Query query, TimeZoneInfo userTimeZone)
    {
        Apply(query.Filter, userTimeZone);
    }

    private static void Apply(QueryFilter filter, TimeZoneInfo userTimeZone)
    {
        foreach (var criterion in filter)
        {
            if (string.Equals(criterion.ColumnName, CreatedTimeColumn, StringComparison.OrdinalIgnoreCase))
            {
                criterion.ClientTimeZone = userTimeZone;
            }

            if (!criterion.Children.IsNullOrEmpty())
            {
                Apply(criterion.Children, userTimeZone);
            }
        }
    }
}
