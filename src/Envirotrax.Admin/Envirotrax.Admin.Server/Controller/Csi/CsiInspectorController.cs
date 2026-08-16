
using DeveloperPartners.SortingFiltering;
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
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? inspectorLicenseNumber, [FromQuery] string? insurancePolicyNumber, [FromQuery] string? userEmail, [FromQuery] string? contactName, CancellationToken cancellationToken)
    {
        var inspectors = await _inspectorService.SearchAsync(pageInfo, query, inspectorLicenseNumber, insurancePolicyNumber, userEmail, contactName, cancellationToken);

        return Ok(inspectors);
    }
}
