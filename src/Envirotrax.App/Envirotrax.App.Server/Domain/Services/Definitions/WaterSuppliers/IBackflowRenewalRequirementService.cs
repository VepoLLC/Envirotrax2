using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IBackflowRenewalRequirementService : IService<BackflowRenewalRequirement, BackflowRenewalRequirementDto>
{
    Task<IEnumerable<BackflowRenewalRequirementDto>> GetAllByWaterSupplierIdAsync(int waterSupplierId, CancellationToken cancellationToken);
}
