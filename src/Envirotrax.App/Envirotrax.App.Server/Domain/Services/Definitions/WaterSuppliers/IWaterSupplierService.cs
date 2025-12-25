
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IWaterSupplierService : IService<WaterSupplier, WaterSupplierDto>
{
    Task<IPagedData<WaterSupplierDto>> GetAllMySuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}