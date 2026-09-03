using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface INotificationSettingService : IService<NotificationSetting, NotificationSettingDto>
{
}
