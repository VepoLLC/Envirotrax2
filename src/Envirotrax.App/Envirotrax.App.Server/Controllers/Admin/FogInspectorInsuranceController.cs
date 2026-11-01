using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspectors/{professionalId}/insurances")]
public class FogInspectorInsuranceController : AdminBaseController
{
    private readonly IProfessionalInsuranceService _insuranceService;

    public FogInspectorInsuranceController(IProfessionalInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var insurances = await _insuranceService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(insurances);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromForm] CreateInsuranceDto insurance)
    {
        insurance.Professional = new ReferencedProfessionalDto
        {
            Id = professionalId
        };

        await using var stream = insurance.File.OpenReadStream();

        var added = await _insuranceService.AddAsync(stream, insurance.File.FileName, insurance);

        return Ok(added);
    }

    [HttpPut("{insuranceId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int insuranceId, [FromBody] ProfessionalInsuranceDto insurance, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, insuranceId, cancellationToken))
        {
            return NotFound();
        }

        insurance.Id = insuranceId;
        insurance.Professional = new ReferencedProfessionalDto
        {
            Id = professionalId
        };

        var updated = await _insuranceService.UpdateAsync(insurance);

        return Ok(updated);
    }

    [HttpGet("{insuranceId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, insuranceId, cancellationToken))
        {
            return NotFound();
        }

        var url = await _insuranceService.GenerateFileUrlAsync(insuranceId, cancellationToken);

        return url == null ? NotFound() : Ok(url);
    }

    [HttpDelete("{insuranceId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, insuranceId, cancellationToken))
        {
            return NotFound();
        }

        var deleted = await _insuranceService.DeleteAsync(insuranceId);

        return deleted == null ? NotFound() : Ok();
    }

    private async Task<bool> BelongsToProfessionalAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        var query = new Query();
        query.Filter.Add(new() { ColumnName = nameof(ProfessionalInsuranceDto.Id), Value = insuranceId.ToString() });

        var owned = await _insuranceService.GetAllByProfessionalAsync(professionalId, new PageInfo(), query, cancellationToken);

        return owned.Data.Any();
    }
}
