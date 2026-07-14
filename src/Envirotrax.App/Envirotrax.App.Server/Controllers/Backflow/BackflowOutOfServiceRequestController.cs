using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/out-of-service-requests")]
[PermissionResource(PermissionType.BackflowOutOfService)]
public class BackflowOutOfServiceRequestController : WaterSupplierProtectedController
{
    private readonly IBackflowOutOfServiceRequestService _service;

    public BackflowOutOfServiceRequestController(IBackflowOutOfServiceRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] PageInfo pageInfo, [FromQuery] Query query,
        [FromQuery] OutOfServiceRequestStatusFilter status = OutOfServiceRequestStatusFilter.AllUncleared,
        [FromQuery] OutOfServiceType? type = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetForWaterSupplierAsync(pageInfo, query, status, type, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/clear")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> ClearAsync(int id, CancellationToken cancellationToken)
    {
        await _service.ClearAsync(id, cancellationToken);
        return NoContent();
    }
}
