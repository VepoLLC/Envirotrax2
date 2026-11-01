using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public record LetterPeriod(DateTime Start, DateTime End, string Name)
{
    private static readonly DateTime OpenStart = new(1990, 1, 1);

    public static LetterPeriod ForExpiring(DateTime now, BackflowExpiringType expiringType)
    {
        return expiringType switch
        {
            BackflowExpiringType.TwoMonths => ForMonth(now, 2, "in 2 Months"),
            BackflowExpiringType.NextMonth => ForMonth(now, 1, "Next Month"),
            _ => ForMonth(now, 0, "This Month")
        };
    }

    public static LetterPeriod ForExpired(DateTime now, BackflowExpiredType expiredType)
    {
        return expiredType switch
        {
            BackflowExpiredType.LastMonth => ForMonth(now, -1, "Last Month"),
            BackflowExpiredType.TwoMonthsAgo => ForMonth(now, -2, "2 Months Ago"),
            BackflowExpiredType.ThreeMonthsAgo => ForMonth(now, -3, "3 Months Ago"),
            _ => ForMonth(now, 0, "This Month")
        };
    }

    public static LetterPeriod ForNonCompliant(DateTime now, BackflowNonCompliantType nonCompliantType)
    {
        return nonCompliantType switch
        {
            BackflowNonCompliantType.ThirtyPlusDays => ForMonthsOverdue(now, 1, "30 Plus Days"),
            BackflowNonCompliantType.SixtyPlusDays => ForMonthsOverdue(now, 2, "60 Plus Days"),
            BackflowNonCompliantType.NinetyPlusDays => ForMonthsOverdue(now, 3, "90 Plus Days"),
            BackflowNonCompliantType.OneTwentyPlusDays => ForMonthsOverdue(now, 4, "120 Plus Days"),
            BackflowNonCompliantType.OneFiftyPlusDays => ForMonthsOverdue(now, 5, "150 Plus Days"),
            BackflowNonCompliantType.OneEightyPlusDays => ForMonthsOverdue(now, 6, "180 Plus Days"),
            BackflowNonCompliantType.PastDueLastMonth => ForMonth(now, -1, "Last Month"),
            _ => ForMonth(now, -2, "2 Months Ago")
        };
    }

    public static LetterPeriod ForImpending(DateTime now, CsiImpendingType impendingType)
    {
        return impendingType switch
        {
            CsiImpendingType.TwoMonths => ForMonth(now, 2, "in 2 Months"),
            CsiImpendingType.NextMonth => ForMonth(now, 1, "Next Month"),
            _ => ForMonth(now, 0, "This Month")
        };
    }

    public static LetterPeriod ForPastDue(DateTime now, CsiPastDueType pastDueType)
    {
        return pastDueType switch
        {
            CsiPastDueType.LastMonth => ForMonth(now, -1, "Last Month"),
            CsiPastDueType.TwoMonthsAgo => ForMonth(now, -2, "2 Months Ago"),
            CsiPastDueType.ThreeMonthsAgo => ForMonth(now, -3, "3 Months Ago"),
            _ => ForMonth(now, 0, "This Month")
        };
    }

    public static LetterPeriod ForNonCompliant(DateTime now, CsiNonCompliantType nonCompliantType)
    {
        return nonCompliantType switch
        {
            CsiNonCompliantType.ThirtyPlusDays => ForMonthsOverdue(now, 1, "30 Plus Days"),
            CsiNonCompliantType.SixtyPlusDays => ForMonthsOverdue(now, 2, "60 Plus Days"),
            CsiNonCompliantType.NinetyPlusDays => ForMonthsOverdue(now, 3, "90 Plus Days"),
            CsiNonCompliantType.OneTwentyPlusDays => ForMonthsOverdue(now, 4, "120 Plus Days"),
            CsiNonCompliantType.OneFiftyPlusDays => ForMonthsOverdue(now, 5, "150 Plus Days"),
            CsiNonCompliantType.OneEightyPlusDays => ForMonthsOverdue(now, 6, "180 Plus Days"),
            CsiNonCompliantType.PastDueLastMonth => ForMonth(now, -1, "Last Month"),
            _ => ForMonth(now, -2, "2 Months Ago")
        };
    }

    private static LetterPeriod ForMonth(DateTime now, int monthOffset, string name)
    {
        var start = new DateTime(now.Year, now.Month, 1).AddMonths(monthOffset);
        var end = start.AddMonths(1).AddSeconds(-1);

        return new LetterPeriod(start, end, name);
    }

    private static LetterPeriod ForMonthsOverdue(DateTime now, int months, string name)
    {
        return new LetterPeriod(OpenStart, now.AddMonths(-months), name);
    }
}
