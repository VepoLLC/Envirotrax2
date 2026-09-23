using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierUserService : IWaterSupplierUserService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public WaterSupplierUserService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<WaterSupplierUserDto>> GetAllAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<WaterSupplierUserDto>($"/api/admin/water-suppliers/{waterSupplierId}/users", pageInfo, query, cancellationToken);
    }
}
