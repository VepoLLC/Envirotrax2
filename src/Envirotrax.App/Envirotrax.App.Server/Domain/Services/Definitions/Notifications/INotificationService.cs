using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface INotificationService : IService<Notification, NotificationDto>
{
    // The matcher already builds a fully-populated model, so saving it directly avoids an
    // unnecessary model -> dto -> model round trip through the generic DTO-based AddAsync.
    Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken);

    Task MarkSentAsync(int id, CancellationToken cancellationToken);
}
