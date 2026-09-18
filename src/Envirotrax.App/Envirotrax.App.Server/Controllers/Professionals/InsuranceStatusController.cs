using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;


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
        var validation = await _insuranceValidationService.ValidateAsync(waterSupplierId, professionalType, cancellationToken);

        return Ok(validation);
    }
}
