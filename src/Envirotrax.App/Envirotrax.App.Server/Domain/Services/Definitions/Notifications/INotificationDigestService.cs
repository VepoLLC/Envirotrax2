using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface INotificationDigestService
{
    Task<List<NotificationDigestSettingDto>> GetPendingAsync(int batchSize, CancellationToken cancellationToken);
    Task ScheduleAsync(int notificationSettingId, CancellationToken cancellationToken);
}
