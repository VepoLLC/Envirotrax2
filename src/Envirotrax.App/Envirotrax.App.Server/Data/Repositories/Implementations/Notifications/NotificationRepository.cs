using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Notifications;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<List<Notification>> AddRangeAsync(List<Notification> notifications)
    {
        Entity.AddRange(notifications);

        await DbContext.SaveChangesAsync();

        return notifications;
    }

    public async Task MarkSentAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        await DbContext.Notifications
            .IgnoreQueryFilters()
            .Where(notification => ids.Contains(notification.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(notification => notification.SentTime, DateTime.UtcNow), cancellationToken);
    }
}
