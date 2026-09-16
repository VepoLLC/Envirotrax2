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

    public async Task MarkSentAsync(int id, CancellationToken cancellationToken)
    {
        await DbContext.Notifications
            .IgnoreQueryFilters()
            .Where(notification => notification.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(notification => notification.SentTime, DateTime.UtcNow), cancellationToken);
    }
}
