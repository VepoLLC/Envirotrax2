using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/settings-copy")]
public class SettingsCopyController : WaterSupplierProtectedController
{
    private readonly ISettingsCopyService _service;

    public SettingsCopyController(ISettingsCopyService service)
    {
        _service = service;
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CanCopyFromParentAsync(CancellationToken cancellationToken)
    {
        var canCopy = await _service.CanCopyFromParentAsync(cancellationToken);

        return Ok(canCopy);
    }

    [HttpPost("{section}")]
    [HasPermission(PermissionAction.CanModify, PermissionType.Settings)]
    public async Task<IActionResult> CopyFromParentAsync(SettingsSection section, CancellationToken cancellationToken)
    {
        await _service.CopyFromParentAsync(section, cancellationToken);

        return Ok();
    }
}
