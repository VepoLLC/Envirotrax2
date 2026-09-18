using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

/// <summary>
/// A separate controller from ProfessionalInsuranceController on purpose: that one is restricted to
/// professional admins, but a BPAT/CSI/FOG transporter submitting work may not be one.
/// </summary>
[Route("api/professionals/insurance-status")]
public class InsuranceStatusController : ProfessionalProtectedController
{
    private readonly IInsuranceValidationService _insuranceValidationService;

    public InsuranceStatusController(IInsuranceValidationService insuranceValidationService)
    {
        _insuranceValidationService = insuranceValidationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] int waterSupplierId,
        [FromQuery] ProfessionalType professionalType,
        CancellationToken cancellationToken)
    {
        // The caller's own ProfessionalId only - there is no way to ask about another professional's
        // insurance through this endpoint.
        var validation = await _insuranceValidationService.ValidateAsync(waterSupplierId, professionalType, cancellationToken);

        return Ok(validation);
    }
}
