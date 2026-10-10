
using Envirotrax.App.Server.Data.Models.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IGeneralSettingsRepository : ITenantSettingsRepository<GeneralSettings>
{
    Task<GeneralSettings?> GetForProfessionalAsync(int waterSupplierId, CancellationToken cancellationToken);
    Task<HashSet<int>> GetRedactingWaterSupplierIdsAsync(IReadOnlyCollection<int> waterSupplierIds, CancellationToken cancellationToken);
}
