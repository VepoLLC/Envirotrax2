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

    public async Task<UpdateResult<NotificationSetting>> UpdateSettingAsync(NotificationSetting model)
    {
        var result = new UpdateResult<NotificationSetting>();

        var setting = await GetTrackedForUpdateAsync(model.Id, default);

        if (setting == null)
        {
            return result;
        }

        // SetValues copies every scalar property in one shot (this entity has ~35 filter/hazard-type
        // flags) — but it would also blank out CreatedById/CreatedTime, since those aren't on the DTO
        // and so are never populated on `model`. Snapshot and restore them across the copy.
        var createdById = setting.CreatedById;
        var createdTime = setting.CreatedTime;

        DbContext.Entry(setting).CurrentValues.SetValues(model);

        setting.CreatedById = createdById;
        setting.CreatedTime = createdTime;

        result.Changes = BuildChangeDescription(setting);

        await DbContext.SaveChangesAsync();

        result.Model = setting;

        return result;
    }
}
