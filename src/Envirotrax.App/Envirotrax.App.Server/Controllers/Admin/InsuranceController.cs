using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/insurances")]
public class InsuranceController : AdminBaseController
{
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly IRecordLogService _recordLogService;

    public InsuranceController(IProfessionalInsuranceService insuranceService, IRecordLogService recordLogService)
    {
        _insuranceService = insuranceService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var insurances = await _insuranceService.GetUnverifiedForAdminAsync(pageInfo, query, cancellationToken);

        return Ok(insurances);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ProfessionalInsuranceDto insurance)
    {
        var existing = await _insuranceService.GetAsync(id, CancellationToken.None);

        if (existing == null)
        {
            return NotFound();
        }

        insurance.Id = id;
        insurance.Professional = existing.Professional;

        var updated = await _insuranceService.UpdateAsync(insurance);

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

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.ProfessionalInsurances, id, cancellationToken);

        return Ok(logs);
    }
}
