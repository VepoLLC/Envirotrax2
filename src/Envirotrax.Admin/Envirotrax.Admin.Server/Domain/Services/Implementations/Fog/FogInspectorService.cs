
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogInspectorService : IFogInspectorService
{
    private const string BaseUrl = "/api/admin/fog/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogInspectorService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<ProfessionalDto>> SearchAsync(FogInspectorSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalDto>(BaseUrl, pageInfo, query, criteria.ToParameters(), cancellationToken);
    }
}
