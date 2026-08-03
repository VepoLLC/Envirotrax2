
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IWaterSupplierUserService
{
    Task<IEnumerable<WaterSupplierUserDto>> GetAllAsync(int waterSupplierId, CancellationToken cancellationToken);
}
