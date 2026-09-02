using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Notifications;

public class NotificationSettingRepository : Repository<NotificationSetting>, INotificationSettingRepository
{
    public NotificationSettingRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<NotificationSetting> GetListQuery()
    {
        return base.GetListQuery().Include(setting => setting.User);
           
    }

    protected override IQueryable<NotificationSetting> GetDetailsQuery()
    {
        return base.GetDetailsQuery().Include(setting => setting.User);
           
    }
}
