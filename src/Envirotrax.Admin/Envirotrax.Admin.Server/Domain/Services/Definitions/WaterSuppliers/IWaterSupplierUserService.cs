using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IWaterSupplierUserService
{
    Task<IPagedData<WaterSupplierUserDto>> GetAllAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
