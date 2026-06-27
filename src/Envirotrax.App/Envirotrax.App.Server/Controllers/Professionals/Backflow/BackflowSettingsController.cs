using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Backflow;

[Route("api/professionals/backflow/settings")]
[HasFeature(FeatureType.BackflowTesting)]
[Authorize(Roles = RoleDefinitions.Professionals.BackflowTester)]
public class BackflowSettingsController : ProfessionalProtectedController
{
    private readonly IBackflowSettingsService _settingsService;

    public BackflowSettingsController(IBackflowSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTestingSettingsAsync([FromQuery] int waterSupplierId, CancellationToken cancellationToken)
    {
        var settings = await _settingsService.GetTestingSettingsAsync(waterSupplierId, cancellationToken);
        return Ok(settings);
    }
}
