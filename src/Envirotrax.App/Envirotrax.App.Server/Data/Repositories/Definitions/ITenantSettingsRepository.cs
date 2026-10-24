
using System.Linq.Expressions;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Repositories.Definitions;

public interface ITenantSettingsRepository<TModel> : IRepository<TModel>
    where TModel : class, ITenantModel
{
    Task<TModel> GetAsync(int waterSupplierId, Expression<Func<TModel, TModel>> selector, CancellationToken cancellationToken = default);

    Task<TModel> GetAsync(Expression<Func<TModel, TModel>> selector, CancellationToken cancellationToken = default);

    Task<TModel> AddOrUpdateAsync(int waterSupplierId, TModel settings);

    Task CopyFromAsync(int targetWaterSupplierId, int sourceWaterSupplierId, SettingsSection section, CancellationToken cancellationToken);
}
