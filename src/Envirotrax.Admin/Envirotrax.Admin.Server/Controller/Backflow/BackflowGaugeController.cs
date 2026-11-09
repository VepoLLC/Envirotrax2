using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/gauges")]
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
        var gauges = await _gaugeService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(gauges);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] BackflowGaugeDto gauge)
    {
        var updated = await _gaugeService.UpdateAsync(id, gauge);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpGet("{id}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var url = await _gaugeService.GetFileUrlAsync(id, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }
}
