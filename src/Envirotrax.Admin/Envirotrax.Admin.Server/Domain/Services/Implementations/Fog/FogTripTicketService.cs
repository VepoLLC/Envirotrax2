using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
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

    public Task<FogTripTicketDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogTripTicketDto>($"{BaseUrl}/{id}", cancellationToken);
    }

    public Task<FogTripTicketDto?> UpdateAsync(int id, int waterSupplierId, FogTripTicketUpdateRequest request, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<FogTripTicketUpdateRequest, FogTripTicketDto>(waterSupplierId, $"{BaseUrl}/{id}", request, cancellationToken);
    }

    public Task<FogTripTicketDto?> UpdateValidationHoldAsync(int id, int waterSupplierId, bool onHold, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<bool, FogTripTicketDto>(waterSupplierId, $"{BaseUrl}/{id}/validation-hold", onHold, cancellationToken);
    }

    public Task<FogTripTicketDto?> UpdateNeedsValidationAsync(int id, int waterSupplierId, bool needsValidation, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<bool, FogTripTicketDto>(waterSupplierId, $"{BaseUrl}/{id}/needs-validation", needsValidation, cancellationToken);
    }

    public Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<RecordLogDto>>($"{BaseUrl}/{id}/logs", cancellationToken);
    }
}
