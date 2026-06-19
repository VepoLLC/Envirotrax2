
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions;

public interface IWaterSupplierService
{
    Task<IPagedData<WaterSupplierDto>> GetAllAsync(PageInfo pageInfo, Query query);
}