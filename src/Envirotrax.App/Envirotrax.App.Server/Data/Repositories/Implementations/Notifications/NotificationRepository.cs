using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Notifications;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public NotificationRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper)
        : base(dbContextSelector)
    {
        _timeZoneHelper = timeZoneHelper;
    }

    protected override IQueryable<Notification> GetListQuery()
    {
        return base.GetListQuery().Include(notification => notification.User);
    }

    public override Task<IEnumerable<Notification>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        CreatedTimeDaySearch.Apply(query, _timeZoneHelper.GetUserTimeZone());

        return base.GetAllAsync(pageInfo, query, cancellationToken);
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

    public async Task<List<Notification>> GetForDigestAsync(
        int userId, NotificationInterval interval, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
    {
        return await Entity
            .Where(notification => notification.UserId == userId
                && notification.Interval == interval
                && notification.CreatedTime >= startUtc
                && notification.CreatedTime < endUtc)
            .OrderBy(notification => notification.CreatedTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<Notification?> SetHiddenAsync(int id, bool hidden, CancellationToken cancellationToken)
    {
        var notification = await GetTrackedForUpdateAsync(id, cancellationToken);

        if (notification == null)
        {
            return null;
        }

        notification.Hidden = hidden;

        await DbContext.SaveChangesAsync(cancellationToken);

        return notification;
    }
}
