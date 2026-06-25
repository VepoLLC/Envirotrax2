using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Fog;

[Route("api/professionals/fog/transportation/disposal-sites")]
[HasFeature(FeatureType.FogTransportation)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.FogTransporter}")]
public class ProfessionalFogTransporterDisposalSitesController : ProfessionalCrudController<FogTransporterDisposalSiteDto>
{
    private readonly IFogTransporterDisposalSiteService _service;

    public ProfessionalFogTransporterDisposalSitesController(IFogTransporterDisposalSiteService service) : base(service)
    {
        _service = service;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _service.GetAvailableAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("available/{disposalSiteId}")]
    public async Task<IActionResult> SetRegistrationAsync(int disposalSiteId, [FromQuery] bool isActive, CancellationToken cancellationToken)
    {
        var result = await _service.SetRegistrationAsync(disposalSiteId, isActive, cancellationToken);
        return Ok(result);
    }
}
