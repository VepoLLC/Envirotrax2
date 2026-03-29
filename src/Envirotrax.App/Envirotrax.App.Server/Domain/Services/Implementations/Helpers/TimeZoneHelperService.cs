
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Helpers;

public class TimeZoneHelperService : ITimeZoneHelperService
{
    private readonly IHttpContextAccessor _contextAcessor;

    public TimeZoneHelperService(IHttpContextAccessor contextAccessor)
    {
        _contextAcessor = contextAccessor;
    }

    public TimeZoneInfo GetUserTimeZone()
    {
        var timeZoneId = _contextAcessor.HttpContext?.Request.Headers["EV-TimeZone"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(timeZoneId))
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }

    public DateTime GetUserLocalTime()
    {
        var timeZone = GetUserTimeZone();
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
    }
}