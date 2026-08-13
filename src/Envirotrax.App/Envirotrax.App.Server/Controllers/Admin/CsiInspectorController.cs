
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspectors")]
public class CsiInspectorController : AdminBaseController
{
    private readonly ICsiInspectorService _inspectorService;

    public CsiInspectorController(ICsiInspectorService inspectorService)
    {
        _inspectorService = inspectorService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? inspectorLicenseNumber, [FromQuery] string? insurancePolicyNumber, [FromQuery] string? userEmail, [FromQuery] string? contactName, CancellationToken cancellationToken)
    {
        var inspectors = await _inspectorService.SearchAsync(inspectorLicenseNumber, insurancePolicyNumber, userEmail, contactName, pageInfo, query, cancellationToken);

        return Ok(inspectors);
    }
}
