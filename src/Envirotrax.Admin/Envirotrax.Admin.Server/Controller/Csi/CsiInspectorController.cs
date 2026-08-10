
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
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? licenseNumber, [FromQuery] string? insuranceNumber, CancellationToken cancellationToken)
    {
        var accounts = await _inspectorService.SearchAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return Ok(accounts);
    }
}
