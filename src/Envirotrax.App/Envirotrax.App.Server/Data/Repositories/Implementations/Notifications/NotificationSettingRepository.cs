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

    public async Task<NotificationSetting?> UpdateSettingAsync(NotificationSetting model)
    {
        var setting = await GetTrackedForUpdateAsync(model.Id, default);

        if (setting == null)
        {
            return null;
        }

        model.WaterSupplierId = setting.WaterSupplierId;

        DbContext.Entry(setting).CurrentValues.SetValues(model);

        await SaveChangesAsync(logData: true);

        return setting;
    }

    public async Task<List<NotificationSetting>> GetCandidateSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await DbContext.NotificationSettings
            .IgnoreQueryFilters()
            .Include(setting => setting.User)
            .Where(setting => setting.WaterSupplierId == waterSupplierId)
            .ToListAsync(cancellationToken);
    }
}
