using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/gauges")]
public class BackflowGaugeController : AdminBaseController
{
    private readonly IBackflowGaugeService _gaugeService;

    public BackflowGaugeController(IBackflowGaugeService gaugeService)
    {
        _gaugeService = gaugeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var gauges = await _gaugeService.GetUnverifiedForAdminAsync(pageInfo, query, cancellationToken);

        return Ok(gauges);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] BackflowGaugeDto gauge)
    {
        var existing = await _gaugeService.GetAsync(id, CancellationToken.None);

        if (existing == null)
        {
            return NotFound();
        }

        gauge.Id = id;
        gauge.Professional = existing.Professional;
        gauge.FilePath = existing.FilePath;

        var updated = await _gaugeService.UpdateAsync(gauge);

        return Ok(updated);
    }

    [HttpGet("{id}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var url = await _gaugeService.GenerateFileUrlAsync(id, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }
}
