using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface IBackflowTestNotificationMatcher
{
    Notification? BuildMatch(NotificationSetting setting, BackflowTest test, BackflowTest? previousTest);
}
