
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Insurances;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Insurances;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/insurances")]
[Authorize(Roles = RoleDefinitions.Professionals.Admin)]
public class ProfessionalInsuranceController : ProfessionalCrudController<ProfessionalInsuranceDto>
{
    private readonly IProfessionalInsuranceService _insuranceService;

    public ProfessionalInsuranceController(IProfessionalInsuranceService service)
        : base(service)
    {
        _insuranceService = service;
    }

    [NonAction]
    public override Task<IActionResult> AddAsync(ProfessionalInsuranceDto dto)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromForm] CreateInsuranceDto insurance)
    {
        await using var stream = insurance.File.OpenReadStream();
        var result = await _insuranceService.AddAsync(stream, insurance.File.FileName, insurance);

        return Ok(result);
    }
}
