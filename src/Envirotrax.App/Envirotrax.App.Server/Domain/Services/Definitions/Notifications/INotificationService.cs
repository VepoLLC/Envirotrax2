using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface INotificationService : IService<Notification, NotificationDto>
{
    // The matcher already builds fully-populated models, so saving them directly avoids an
    // unnecessary model -> dto -> model round trip through the generic DTO-based AddAsync.
    Task<List<Notification>> AddAsync(List<Notification> notifications, CancellationToken cancellationToken);

    Task MarkSentAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}
