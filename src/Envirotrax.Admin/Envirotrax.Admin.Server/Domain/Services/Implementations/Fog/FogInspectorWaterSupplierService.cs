using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogInspectorWaterSupplierService : IFogInspectorWaterSupplierService
{
    private const string BaseUrl = "/api/admin/fog/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogInspectorWaterSupplierService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<ProfessionalWaterSupplierDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalWaterSupplierDto>($"{BaseUrl}/{professionalId}/water-suppliers", pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalWaterSupplierDto?> UpdateAsync(int professionalId, int waterSupplierId, ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalWaterSupplierDto, ProfessionalWaterSupplierDto>($"{BaseUrl}/{professionalId}/water-suppliers/{waterSupplierId}", registration, cancellationToken);
    }
}
