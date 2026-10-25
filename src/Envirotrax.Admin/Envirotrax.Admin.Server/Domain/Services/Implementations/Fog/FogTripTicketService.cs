using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketService : IFogTripTicketService
{
    private const string BaseUrl = "/api/admin/fog/trip-tickets";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogTripTicketService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<FogTripTicketDto>> SearchAsync(FogTripTicketSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogTripTicketDto>(BaseUrl, pageInfo, query, criteria.ToParameters(), cancellationToken);
    }
}
