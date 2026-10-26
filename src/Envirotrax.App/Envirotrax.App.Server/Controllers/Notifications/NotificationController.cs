using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Notifications;

[Route("api/notifications")]
[PermissionResource(PermissionType.Notifications)]
public class NotificationController : WaterSupplierProtectedController
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var notifications = await _service.GetAllAsync(pageInfo, query, cancellationToken);
        return Ok(notifications);
    }

    [HttpPut("{id}/hidden")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> SetHiddenAsync(int id, SetNotificationHiddenRequest request, CancellationToken cancellationToken)
    {
        var notification = await _service.SetHiddenAsync(id, request.Hidden, cancellationToken);

        if (notification == null)
        {
            return NotFound();
        }

        return Ok(notification);
    }
}
