using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IBackflowSettingsRepository : ITenantSettingsRepository<BackflowSettings>
{
    Task<BackflowTestingSettingsDto?> GetTestingSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<BackflowTestingSettingsDto?> GetTestingSettingsByWaterSupplierAsync(int waterSupplierId, CancellationToken cancellationToken);
}
