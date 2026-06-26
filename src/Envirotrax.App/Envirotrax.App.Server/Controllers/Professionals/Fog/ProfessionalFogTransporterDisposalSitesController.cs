using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Fog;

[Route("api/professionals/fog/transportation/disposal-sites")]
[HasFeature(FeatureType.FogTransportation)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.FogTransporter}")]
public class ProfessionalFogTransporterDisposalSitesController : ProfessionalProtectedController
{
    private readonly IFogDisposalSiteService _disposalSiteService;
    private readonly IFogTransporterDisposalSiteService _registrationService;

    public ProfessionalFogTransporterDisposalSitesController(
        IFogDisposalSiteService disposalSiteService,
        IFogTransporterDisposalSiteService registrationService)
    {
        _disposalSiteService = disposalSiteService;
        _registrationService = registrationService;
    }

    // Master list of all (non-deleted) disposal sites.
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _disposalSiteService.GetActiveAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    // Disposal sites the current professional has registered (reused for FOG trip tickets).
    [HttpGet("registered")]
    public async Task<IActionResult> GetRegisteredAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _registrationService.GetRegisteredDisposalSitesAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    // Select (isActive=true) or unselect (isActive=false) a disposal site for the current professional.
    // Registering is restricted to professional Admins — FOG Transporters can view but not add/remove.
    [HttpPut("{disposalSiteId}/registration")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public async Task<IActionResult> SetRegistrationAsync(int disposalSiteId, [FromQuery] bool isActive, CancellationToken cancellationToken)
    {
        await _registrationService.SetRegistrationAsync(disposalSiteId, isActive, cancellationToken);
        return Ok();
    }
}
