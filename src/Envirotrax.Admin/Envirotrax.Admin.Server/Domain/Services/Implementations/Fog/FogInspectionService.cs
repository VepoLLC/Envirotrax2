using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : IFogInspectionService
{
    private const string BaseUrl = "/api/admin/fog/inspections";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogInspectionService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<FogInspectionDto>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogInspectionDto>(BaseUrl, pageInfo, query, cancellationToken);
    }
}
