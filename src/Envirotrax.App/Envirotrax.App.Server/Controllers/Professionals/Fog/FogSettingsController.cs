using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Fog;

[Route("api/professionals/fog/settings")]
[HasFeature(FeatureType.FogTransportation)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.FogTransporter}")]
public class FogSettingsController : ProfessionalProtectedController
{
    private readonly IFogSettingsService _settingsService;

    public FogSettingsController(IFogSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("{waterSupplierId}")]
    public async Task<IActionResult> GetSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var settings = await _settingsService.GetSettingsAsync(waterSupplierId, cancellationToken);
        return Ok(settings);
    }
}
