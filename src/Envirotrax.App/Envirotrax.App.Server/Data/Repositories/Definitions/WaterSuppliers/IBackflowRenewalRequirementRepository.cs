using Envirotrax.App.Server.Data.Models.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IBackflowRenewalRequirementRepository : IRepository<BackflowRenewalRequirement>
{
    Task<IEnumerable<BackflowRenewalRequirement>> GetAllByWaterSupplierIdAsync(int waterSupplierId, CancellationToken cancellationToken);
}
