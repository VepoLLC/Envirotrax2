
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspectors")]
public class CsiInspectorController : AdminBaseController
{
    private readonly ICsiInspectorService _inspectorService;

    public CsiInspectorController(ICsiInspectorService inspectorService)
    {
        _inspectorService = inspectorService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? licenseNumber, [FromQuery] string? insuranceNumber, CancellationToken cancellationToken)
    {
        var accounts = await _inspectorService.SearchAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return Ok(accounts);
    }

    [HttpGet("{professionalId}")]
    public async Task<IActionResult> GetDetailsAsync(int professionalId, [FromQuery] int? userId, CancellationToken cancellationToken)
    {
        var details = await _inspectorService.GetDetailsAsync(professionalId, userId, cancellationToken);

        return details == null ? NotFound() : Ok(details);
    }

    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateDetailsAsync(int professionalId, [FromBody] CsiInspectorAccountDetailsDto details, CancellationToken cancellationToken)
    {
        var updated = await _inspectorService.UpdateDetailsAsync(professionalId, details, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
