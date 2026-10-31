
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspectors")]
public class FogInspectorController : AdminBaseController
{
    private readonly IFogInspectorService _inspectorService;

    public FogInspectorController(IFogInspectorService inspectorService)
    {
        _inspectorService = inspectorService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogInspectorSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspectors = await _inspectorService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(inspectors);
    }

    [HttpGet("{professionalId}")]
    public async Task<IActionResult> GetAsync(int professionalId, CancellationToken cancellationToken)
    {
        var inspector = await _inspectorService.GetAsync(professionalId, cancellationToken);

        return inspector == null ? NotFound() : Ok(inspector);
    }

    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, [FromBody] ProfessionalDto professional, CancellationToken cancellationToken)
    {
        var updated = await _inspectorService.UpdateAsync(professionalId, professional, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
