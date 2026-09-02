
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

/// <summary>
/// Public directory of professionals registered with a water supplier. This is the V2 home of V1's
/// registrations.aspx, which the marketing site links to, so every endpoint is anonymous and
/// returns only the contact details a company agreed to publish.
/// </summary>
[AllowAnonymous]
[Route("api/registered-professionals")]
public class RegisteredProfessionalController : EnvirotraxBaseController
{
    private readonly IRegisteredProfessionalService _registeredProfessionalService;

    public RegisteredProfessionalController(IRegisteredProfessionalService registeredProfessionalService)
    {
        _registeredProfessionalService = registeredProfessionalService;
    }

    /// <summary>
    /// Water suppliers that publicly run the program this professional type serves.
    /// </summary>
    [HttpGet("water-suppliers")]
    public async Task<IActionResult> GetWaterSuppliersAsync(
        [FromQuery] ProfessionalType professionalType,
        CancellationToken cancellationToken)
    {
        var suppliers = await _registeredProfessionalService.GetWaterSuppliersAsync(professionalType, cancellationToken);
        return Ok(suppliers);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] int waterSupplierId,
        [FromQuery] ProfessionalType professionalType,
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        CancellationToken cancellationToken)
    {
        var results = await _registeredProfessionalService.SearchAsync(
            waterSupplierId, professionalType, pageInfo, query, cancellationToken);

        return Ok(results);
    }
}
