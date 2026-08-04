using Envirotrax.Admin.Server.Domain.DataTransferObjects.GoogleMaps;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.GoogleMaps;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.GoogleMaps;

public class GoogleMapsService : IGoogleMapsService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public GoogleMapsService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<GoogleMapsApiKeyDto> GetApiKeyAsync(CancellationToken cancellationToken)
    {
        var apiKey = await _apiClient.GetAsync<GoogleMapsApiKeyDto>("/api/admin/google-maps/api-key", cancellationToken);

        return apiKey ?? new GoogleMapsApiKeyDto();
    }
}
