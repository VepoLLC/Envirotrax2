using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Fog;

[Route("api/professionals/fog/trip-tickets")]
[HasFeature(FeatureType.FogTransportation)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.FogTransporter}")]
public class ProfessionalFogTripTicketController : ProfessionalProtectedController
{
    private readonly IFogTripTicketService _fogService;

    public ProfessionalFogTripTicketController(IFogTripTicketService fogService)
    {
        _fogService = fogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        [FromQuery] int? waterSupplierId,
        CancellationToken ct)
    {
        var result = await _fogService.SearchForProfessionalAsync(pageInfo, query, waterSupplierId, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken ct)
    {
        var result = await _fogService.GetAsync(id, ct);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _fogService.DeleteAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}
