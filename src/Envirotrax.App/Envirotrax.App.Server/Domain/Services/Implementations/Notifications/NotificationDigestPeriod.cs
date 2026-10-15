using System.Globalization;
using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public record NotificationDigestPeriod(NotificationInterval Interval, DateTime Start, DateTime End)
{
    private static readonly TimeZoneInfo CentralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    private static readonly TimeSpan SendTimeOfDay = TimeSpan.FromHours(7);
    private static readonly TimeSpan LateSendDelay = TimeSpan.FromMinutes(5);

    public DateTime StartUtc => ToUtc(Start);

    public DateTime EndUtc => ToUtc(End);

    public string Description => Interval switch
    {
        NotificationInterval.EndOfDay => FormatDate(Start),
        NotificationInterval.EndOfWeek => $"{FormatDate(Start)} - {FormatDate(End.AddDays(-1))}",
        _ => Start.ToString("MMMM yyyy", CultureInfo.InvariantCulture)
    };

    public static NotificationDigestPeriod GetLastClosed(NotificationInterval interval, DateTime utcNow)
    {
        var today = ToCentralTime(utcNow).Date;

        var end = interval switch
        {
            NotificationInterval.EndOfDay => today,
            NotificationInterval.EndOfWeek => today.AddDays(-(int)today.DayOfWeek),
            NotificationInterval.EndOfMonth => new DateTime(today.Year, today.Month, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, "Immediate notifications have no digest period.")
        };

        var start = interval switch
        {
            NotificationInterval.EndOfDay => end.AddDays(-1),
            NotificationInterval.EndOfWeek => end.AddDays(-7),
            _ => end.AddMonths(-1)
        };

        return new NotificationDigestPeriod(interval, start, end);
    }

    public static DateTime ToCentralTime(DateTime utcTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, CentralTimeZone);
    }

    public DateTime GetSendTimeUtc(DateTime utcNow)
    {
        var sendTime = ToUtc(End.Add(SendTimeOfDay));
        var earliestSendTime = utcNow.Add(LateSendDelay);

        return sendTime > earliestSendTime ? sendTime : earliestSendTime;
    }

    private static DateTime ToUtc(DateTime centralTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(centralTime, CentralTimeZone);
    }

    private static string FormatDate(DateTime date)
    {
        return date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
    }
}
