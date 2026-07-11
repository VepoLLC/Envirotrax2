using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
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
    private readonly IFogVehicleService _vehicleService;
    private readonly IFogTransporterDisposalSiteService _disposalSiteService;
    private readonly IProfessionalService _professionalService;

    public ProfessionalFogTripTicketController(
        IFogTripTicketService fogService,
        IFogVehicleService vehicleService,
        IFogTransporterDisposalSiteService disposalSiteService,
        IProfessionalService professionalService)
    {
        _fogService = fogService;
        _vehicleService = vehicleService;
        _disposalSiteService = disposalSiteService;
        _professionalService = professionalService;
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
            return Ok(Array.Empty<FogLookupItemDto>());
        }

        var options = new List<FogLookupItemDto> { new(current.Id, current.Name) };

        if (current.ParentId == null)
        {
            var subAccounts = await _professionalService.GetSubAccountsAsync(ct);
            options.AddRange(subAccounts.Select(p => new FogLookupItemDto(p.Id, p.Name)));
        }

        return Ok(options);
    }

    [HttpGet("lookup/vehicles")]
    public async Task<IActionResult> GetVehicles(CancellationToken ct)
    {
        var result = await _vehicleService.GetAsOptionsAsync(ct);
        return Ok(result);
    }

    [HttpGet("lookup/disposal-sites")]
    public async Task<IActionResult> GetDisposalSites(CancellationToken ct)
    {
        var result = await _disposalSiteService.GetRegisteredDisposalSitesAsync(
            new PageInfo { PageSize = 1000 }, new Query(), ct);

        var options = result.Data
            .Select(s => new FogLookupItemDto(s.Id, s.Name))
            .OrderBy(x => x.Text)
            .ToList();

        return Ok(options);
    }
}
