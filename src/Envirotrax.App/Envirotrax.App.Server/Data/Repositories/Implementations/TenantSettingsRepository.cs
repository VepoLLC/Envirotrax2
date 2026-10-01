
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

public abstract class TenantSettingsRepository<TModel> : Repository<TModel>, ITenantSettingsRepository<TModel>
    where TModel : class, ITenantModel, new()
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

    public async Task CopyFromAsync(int targetWaterSupplierId, int sourceWaterSupplierId, SettingsSection section, CancellationToken cancellationToken)
    {
        var source = await DbContext.Set<TModel>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleOrDefaultAsync(s => s.WaterSupplierId == sourceWaterSupplierId, cancellationToken);

        if (source == null)
        {
            throw new AppValidationException("The parent water supplier account has no settings to copy.");
        }

        var target = await Entity.SingleOrDefaultAsync(s => s.WaterSupplierId == targetWaterSupplierId, cancellationToken);

        if (target == null)
        {
            target = new TModel { WaterSupplierId = targetWaterSupplierId };
            Entity.Add(target);
        }

        CopyValues(source, target, section);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected abstract void CopyValues(TModel source, TModel target, SettingsSection section);
}
