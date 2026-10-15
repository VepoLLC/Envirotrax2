using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;

public interface INotificationSettingRepository : IRepository<NotificationSetting>
{
    Task<NotificationSetting?> UpdateSettingAsync(NotificationSetting model);
    Task<List<NotificationSetting>> GetCandidateSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);
    Task<List<NotificationSetting>> GetPendingDigestsAsync(NotificationInterval interval, DateTime startUtc, DateTime endUtc, int batchSize, CancellationToken cancellationToken);
    Task<NotificationSetting?> GetForDigestAsync(int id, CancellationToken cancellationToken);
}
