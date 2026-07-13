using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
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
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;

    public ProfessionalFogTripTicketController(
        IFogTripTicketService fogService,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService)
    {
        _fogService = fogService;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
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

    [HttpGet("lookup/transporters")]
    public async Task<IActionResult> GetTransporters(CancellationToken ct)
    {
        var current = await _professionalService.GetLoggedInProfessionalAsync(ct);

        if (current == null)
        {
            return Ok(Array.Empty<ProfessionalUserDto>());
        }

        var result = await _professionalUserService.GetAllByProfessionalAsync(
            current.Id, new PageInfo { PageSize = 1000 }, new Query(), ct, pu => pu.IsFogTransporter);

        return Ok(result.Data);
    }
}
