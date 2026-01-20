using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class GeneralSettingsRepository : Repository<GeneralSettings>, IGeneralSettingsRepository
{
    public GeneralSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
