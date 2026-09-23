
using Envirotrax.Admin.Server.Domain.Services.Definitions.GoogleMaps;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.GoogleMaps;

/// <summary>
/// Serves the public Google Maps API key (from Admin's own configuration / Key Vault) so the shared
/// vp-map component can load the Maps SDK from the Admin client (which resolves /api/google-maps/api-key).
/// </summary>
[Route("api/google-maps")]
public class GoogleMapsController : AdminBaseController
{
    private readonly IGoogleMapsService _googleMapsService;

    public GoogleMapsController(IGoogleMapsService googleMapsService)
    {
        _googleMapsService = googleMapsService;
    }

    [HttpGet("api-key")]
    public async Task<IActionResult> GetApiKeyAsync(CancellationToken cancellationToken)
    {
        var apiKey = await _googleMapsService.GetApiKeyAsync(cancellationToken);

        return Ok(apiKey);
    }
}
