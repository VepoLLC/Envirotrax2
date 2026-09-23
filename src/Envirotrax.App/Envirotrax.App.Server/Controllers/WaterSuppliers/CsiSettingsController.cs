using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/csi-settings")]
public class CsiSettingsController : WaterSupplierCrudController<CsiSettingsDto>
{
    public CsiSettingsController(ICsiSettingsService service)
        : base(service)
    {
    }

    [HasPermission(PermissionAction.CanModify, PermissionType.Settings)]
    public override Task<IActionResult> AddAsync(CsiSettingsDto dto)
    {
        return base.AddAsync(dto);
    }

    [HasPermission(PermissionAction.CanModify, PermissionType.Settings)]
    public override Task<IActionResult> UpdateAsync(int id, CsiSettingsDto dto)
    {
        return base.UpdateAsync(id, dto);
    }

    [HasPermission(PermissionAction.CanDelete, PermissionType.Settings)]
    public override Task<IActionResult> DeleteAsync(int id)
    {
        return base.DeleteAsync(id);
    }
}
