
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Repositories.Definitions;

public interface ITenantSettingsRepository<TModel> : IRepository<TModel>
    where TModel : class, ITenantModel
{
    Task<TModel> AddOrUpdateAsync(int waterSupplierId, TModel settings);

    Task CopyFromAsync(int targetWaterSupplierId, int sourceWaterSupplierId, SettingsSection section, CancellationToken cancellationToken);
}
