
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Repositories.Definitions;

public interface ITenantSettingsRepository<TModel> : IRepository<TModel>
    where TModel : class, ITenantModel
{
    Task<TModel> AddOrUpdateAsync(int waterSupplierId, TModel settings);
}
