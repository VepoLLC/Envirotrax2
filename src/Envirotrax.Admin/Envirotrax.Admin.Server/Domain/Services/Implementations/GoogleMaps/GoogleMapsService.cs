using Envirotrax.Admin.Server.Domain.DataTransferObjects.GoogleMaps;
using Envirotrax.Admin.Server.Domain.Services.Definitions.GoogleMaps;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.GoogleMaps;

public class GoogleMapsService : IGoogleMapsService
{
    private readonly IConfiguration _configuration;

    public GoogleMapsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<GoogleMapsApiKeyDto> GetApiKeyAsync(CancellationToken cancellationToken)
    {
        // Read the public Maps key straight from Admin's own configuration (Azure Key Vault) — no round
        // trip to Envirotrax.App is needed now that the key is provisioned for this project.
        var apiKey = _configuration["GoogleMaps:PublicApiKey"];

        return Task.FromResult(new GoogleMapsApiKeyDto { ApiKey = apiKey });
    }
}
