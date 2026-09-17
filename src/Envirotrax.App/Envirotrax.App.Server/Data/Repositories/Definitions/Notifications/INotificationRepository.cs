using Envirotrax.App.Server.Data.Models.Notifications;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;

public interface INotificationRepository : IRepository<Notification>
{
    // One insert for the whole batch — saving notifications one by one cost a database
    // round trip each, which dominated the time of a multi-item checkout.
    Task<List<Notification>> AddRangeAsync(List<Notification> notifications);

    Task MarkSentAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}
