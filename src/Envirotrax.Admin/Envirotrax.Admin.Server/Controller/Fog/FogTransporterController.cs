
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/transporters")]
public class FogTransporterController : AdminBaseController
{
    private readonly IFogTransporterService _transporterService;

    public FogTransporterController(IFogTransporterService transporterService)
    {
        _transporterService = transporterService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] FogTransporterSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var transporters = await _transporterService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(transporters);
    }

    [HttpGet("{professionalId}")]
    public async Task<IActionResult> GetAsync(int professionalId, CancellationToken cancellationToken)
    {
        var transporter = await _transporterService.GetAsync(professionalId, cancellationToken);

        return transporter == null ? NotFound() : Ok(transporter);
    }

    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, [FromBody] ProfessionalDto professional, CancellationToken cancellationToken)
    {
        var updated = await _transporterService.UpdateAsync(professionalId, professional, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
