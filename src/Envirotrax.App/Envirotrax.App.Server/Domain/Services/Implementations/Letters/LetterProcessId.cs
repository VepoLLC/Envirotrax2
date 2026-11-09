using System.Globalization;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public static class LetterProcessId
{
    private const string Format = "yyyy-MM-dd HH-mm-ss";

    public static string FromProcessDate(DateTime processDate)
    {
        return processDate.ToString(Format, CultureInfo.InvariantCulture);
    }

    public static bool TryGetProcessDate(string processId, out DateTime processDate)
    {
        return DateTime.TryParseExact(processId, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out processDate);
    }
}
