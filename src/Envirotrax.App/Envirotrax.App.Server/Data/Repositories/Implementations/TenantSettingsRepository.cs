
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

public abstract class TenantSettingsRepository<TModel> : Repository<TModel>, ITenantSettingsRepository<TModel>
    where TModel : class, ITenantModel
{
    public TenantSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<TModel> AddOrUpdateAsync(int waterSupplierId, TModel settings)
    {
        settings.WaterSupplierId = waterSupplierId;

        var existing = await Entity.SingleOrDefaultAsync(s => s.WaterSupplierId == waterSupplierId);

        if (existing == null)
        {
            return await AddAsync(settings);
        }

        DbContext.Entry(existing).CurrentValues.SetValues(settings);

        await DbContext.SaveChangesAsync();

        return existing;
    }
}
