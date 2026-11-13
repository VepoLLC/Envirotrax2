using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/insurances")]
public class InsuranceController : AdminBaseController
{
    private readonly IProfessionalInsuranceService _insuranceService;

    public InsuranceController(IProfessionalInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var insurances = await _insuranceService.GetUnverifiedAsync(pageInfo, query, cancellationToken);

        return Ok(insurances);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalInsuranceDto insurance)
    {
        var updated = await _insuranceService.UpdateForAdminAsync(id, insurance);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpGet("{id}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var url = await _insuranceService.GenerateFileUrlAsync(id, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }
}
