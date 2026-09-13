using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;

public interface INotificationSettingRepository : IRepository<NotificationSetting>
{
    Task<UpdateResult<NotificationSetting>> UpdateSettingAsync(NotificationSetting model);
}
