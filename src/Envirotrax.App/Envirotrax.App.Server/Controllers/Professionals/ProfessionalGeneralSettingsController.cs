using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/general-settings")]
public class ProfessionalGeneralSettingsController : ProfessionalProtectedController
{
    private readonly IGeneralSettingsService _settingsService;

    public ProfessionalGeneralSettingsController(IGeneralSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("{waterSupplierId}")]
    public async Task<IActionResult> GetAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var settings = await _settingsService.GetForProfessionalAsync(waterSupplierId, cancellationToken);
        return Ok(settings);
    }
}
