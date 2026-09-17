using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;

public interface INotificationSettingRepository : IRepository<NotificationSetting>
{
    Task<NotificationSetting?> UpdateSettingAsync(NotificationSetting model);
}
