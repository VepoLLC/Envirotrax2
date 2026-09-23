using Envirotrax.Admin.Server.Domain.DataTransferObjects.GoogleMaps;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.GoogleMaps;

public interface IGoogleMapsService
{
    Task<GoogleMapsApiKeyDto> GetApiKeyAsync(CancellationToken cancellationToken);
}
