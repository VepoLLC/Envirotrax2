using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class CsiSettingsRepository : Repository<CsiSettings>, ICsiSettingsRepository
{
    public CsiSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
