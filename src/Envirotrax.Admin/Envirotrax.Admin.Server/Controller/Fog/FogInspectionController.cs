using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspections")]
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
        var inspections = await _inspectionService.SearchAsync(pageInfo, query, paymentStatus, totalCapacityRange, cancellationToken);

        return Ok(inspections);
    }
}
