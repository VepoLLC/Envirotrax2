using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Licenses;

[Route("api/insurances")]
public class InsuranceController : AdminBaseController
{
    private readonly IInsuranceService _insuranceService;

    public InsuranceController(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var insurances = await _insuranceService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(insurances);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalInsuranceDto insurance)
    {
        var updated = await _insuranceService.UpdateAsync(id, insurance);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpGet("{id}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var url = await _insuranceService.GetFileUrlAsync(id, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _insuranceService.GetLogsAsync(id, cancellationToken);

        return Ok(logs);
    }
}
