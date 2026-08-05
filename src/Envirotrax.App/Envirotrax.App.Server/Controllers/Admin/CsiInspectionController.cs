
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspections")]
public class CsiInspectionController : AdminBaseController
{
    private readonly ICsiInspectionService _inspectionService;

    public CsiInspectionController(ICsiInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        [FromQuery] CsiPaymentStatus? paymentStatus,
        CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.SearchForAdminAsync(pageInfo, query, paymentStatus, cancellationToken);

        return Ok(inspections);
    }
}
