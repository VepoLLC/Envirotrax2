using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspections")]
public class FogInspectionController : AdminBaseController
{
    private readonly IFogInspectionService _inspectionService;

    public FogInspectionController(IFogInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        [FromQuery] FogPaymentStatus? paymentStatus,
        [FromQuery] FogTotalCapacityRange? totalCapacityRange,
        CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.SearchForAdminAsync(pageInfo, query, paymentStatus, totalCapacityRange, cancellationToken);

        return Ok(inspections);
    }
}
