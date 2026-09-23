using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Notifications;

[Route("api/notification-settings")]
[PermissionResource(PermissionType.Notifications)]
public class NotificationSettingController : WaterSupplierCrudController<NotificationSettingDto>
{
    public NotificationSettingController(INotificationSettingService service)
        : base(service)
    {
    }

    [HasPermission(PermissionAction.CanModify)]
    public override Task<IActionResult> DeleteAsync(int id)
    {
        return base.DeleteAsync(id);
    }
}
