
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.Configuration;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierService : IWaterSupplierService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public WaterSupplierService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<WaterSupplierDto>> GetAllAsync(PageInfo pageInfo, Query query)
    {
        return _apiClient.GetAsync<WaterSupplierDto>("/api/admin/water-suppliers", pageInfo, query);
    }
}