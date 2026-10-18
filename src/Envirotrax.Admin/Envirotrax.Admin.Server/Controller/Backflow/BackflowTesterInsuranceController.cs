using System.ComponentModel.DataAnnotations;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/insurances")]
public class BackflowTesterInsuranceController : AdminBaseController
{
    private readonly IBackflowTesterInsuranceService _insuranceService;

    public BackflowTesterInsuranceController(IBackflowTesterInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var insurances = await _insuranceService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(insurances);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromForm] ProfessionalInsuranceDto insurance, [Required] IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var added = await _insuranceService.AddAsync(professionalId, insurance, stream, file.FileName, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{insuranceId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int insuranceId, [FromBody] ProfessionalInsuranceDto insurance, CancellationToken cancellationToken)
    {
        var updated = await _insuranceService.UpdateAsync(professionalId, insuranceId, insurance, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpGet("{insuranceId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        var url = await _insuranceService.GetFileUrlAsync(professionalId, insuranceId, cancellationToken);

        return url == null ? NotFound() : Ok(url);
    }

    [HttpDelete("{insuranceId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        await _insuranceService.DeleteAsync(professionalId, insuranceId, cancellationToken);

        return Ok();
    }
}
