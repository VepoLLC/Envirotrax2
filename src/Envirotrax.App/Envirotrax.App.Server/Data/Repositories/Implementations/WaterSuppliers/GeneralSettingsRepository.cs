using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class GeneralSettingsRepository : TenantSettingsRepository<GeneralSettings>, IGeneralSettingsRepository
{
    public GeneralSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void CopyValues(GeneralSettings source, GeneralSettings target, SettingsSection section)
    {
        if (section != SettingsSection.General)
        {
            throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }

        source.WaterSupplierId = target.WaterSupplierId;

        DbContext.Entry(target).CurrentValues.SetValues(source);
    }
}
