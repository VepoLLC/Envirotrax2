using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

public abstract class ProfessionalPaymentBypassController : WaterSupplierProtectedController
{
    private readonly IProfessionalUserService _userService;

    protected abstract ProfessionalType ProfessionalType { get; }

    protected ProfessionalPaymentBypassController(IProfessionalUserService userService)
    {
        _userService = userService;
    }

    [HttpPut("{userId}/payment-bypass")]
    [HasFeature(FeatureType.ManagePaymentBypass)]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateAsync(int professionalId, int userId, [FromBody] ProfessionalPaymentBypassDto paymentBypass)
    {
        var updated = await _userService.UpdatePaymentBypassAsync(professionalId, userId, ProfessionalType, paymentBypass.IsPaymentBypassed);

        return updated == null ? NotFound() : Ok(updated);
    }
}
