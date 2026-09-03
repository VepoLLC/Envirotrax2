
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspectors")]
public class CsiInspectorController : AdminBaseController
{
    private readonly ICsiInspectorAccountService _inspectorService;

    public CsiInspectorController(ICsiInspectorAccountService inspectorService)
    {
        _inspectorService = inspectorService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? licenseNumber, [FromQuery] string? insuranceNumber, CancellationToken cancellationToken)
    {
        var accounts = await _inspectorService.SearchForAdminAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return Ok(accounts);
    }

    [HttpGet("{professionalId}")]
    public async Task<IActionResult> GetDetailsAsync(int professionalId, [FromQuery] int? userId, CancellationToken cancellationToken)
    {
        var details = await _inspectorService.GetDetailsForAdminAsync(professionalId, userId, cancellationToken);

        return details == null ? NotFound() : Ok(details);
    }

    [HttpPut("{professionalId}")]
    public async Task<IActionResult> UpdateDetailsAsync(int professionalId, [FromBody] CsiInspectorAccountDetailsDto details, CancellationToken cancellationToken)
    {
        var updated = await _inspectorService.UpdateDetailsForAdminAsync(professionalId, details, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }
}
