
namespace Envirotrax.App.Server.Domain.Services.Definitions.Helpers;

public interface ITimeZoneHelperService
{
    TimeZoneInfo GetUserTimeZone();
    DateTime GetUserLocalTime();
}