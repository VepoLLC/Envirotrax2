using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;

public interface INotificationRepository : IRepository<Notification>
{
    Task MarkSentAsync(int id, CancellationToken cancellationToken);
}
