
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspections")]
public class CsiInspectionController : AdminBaseController
{
    private readonly ICsiInspectionService _inspectionService;

    public CsiInspectionController(ICsiInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo,[FromQuery] Query query,[FromQuery] CsiPaymentStatus? paymentStatus,[FromQuery] int? inspectorId, CancellationToken cancellationToken)  
    {
        var inspections = await _inspectionService.SearchAsync(pageInfo, query, paymentStatus, inspectorId, cancellationToken);

        return Ok(inspections);
    }
}
