
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Admin FOG transporter search and account details. A route shell only, like the admin FOG inspector controller:
/// the search reuses IFogTransporterService.SearchAsync, and the account is read and saved through
/// IProfessionalService, whose repository protects AccountBalance on update.
/// </summary>
[Route("api/admin/fog/transporters")]
public class FogTransporterController : AdminBaseController
{
    private readonly IFogTransporterService _transporterService;
    private readonly IProfessionalService _professionalService;

    public FogTransporterController(IFogTransporterService transporterService, IProfessionalService professionalService)
    {
        _transporterService = transporterService;
        _professionalService = professionalService;
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
        var professional = await _professionalService.GetAsync(professionalId, cancellationToken);

        return professional == null ? NotFound() : Ok(professional);
    }

    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, [FromBody] ProfessionalDto professional, CancellationToken cancellationToken)
    {
        if (!await _professionalService.ExistsAsync(professionalId, cancellationToken))
        {
            return NotFound();
        }

        professional.Id = professionalId;

        var updated = await _professionalService.UpdateAsync(professional);

        return Ok(updated);
    }
}
