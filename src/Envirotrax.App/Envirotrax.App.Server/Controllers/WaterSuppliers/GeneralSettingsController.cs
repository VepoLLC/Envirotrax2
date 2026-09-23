using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/general-settings")]
public class GeneralSettingsController : WaterSupplierCrudController<GeneralSettingsDto>
{
    public GeneralSettingsController(IGeneralSettingsService service)
        : base(service)
    {
    }

    [HasPermission(PermissionAction.CanModify, PermissionType.Settings)]
    public override Task<IActionResult> AddAsync(GeneralSettingsDto dto)
    {
        return base.AddAsync(dto);
    }

    [HasPermission(PermissionAction.CanModify, PermissionType.Settings)]
    public override Task<IActionResult> UpdateAsync(int id, GeneralSettingsDto dto)
    {
        return base.UpdateAsync(id, dto);
    }

    [HasPermission(PermissionAction.CanDelete, PermissionType.Settings)]
    public override Task<IActionResult> DeleteAsync(int id)
    {
        return base.DeleteAsync(id);
    }
}
