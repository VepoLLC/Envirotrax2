
using DeveloperPartners.SortingFiltering;
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
}
