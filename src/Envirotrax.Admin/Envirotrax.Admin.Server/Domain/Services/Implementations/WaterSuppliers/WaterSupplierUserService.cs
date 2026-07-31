
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

    public async Task<IEnumerable<WaterSupplierUserDto>> GetAllAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var users = await _apiClient.GetAsync<IEnumerable<WaterSupplierUserDto>>($"/api/admin/water-suppliers/{waterSupplierId}/users", cancellationToken);

        return users ?? [];
    }
}
