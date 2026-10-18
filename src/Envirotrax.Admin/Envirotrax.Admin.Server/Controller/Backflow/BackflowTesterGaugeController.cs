using System.ComponentModel.DataAnnotations;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/gauges")]
public class BackflowTesterGaugeController : AdminBaseController
{
    private readonly IBackflowTesterGaugeService _gaugeService;

    public BackflowTesterGaugeController(IBackflowTesterGaugeService gaugeService)
    {
        _gaugeService = gaugeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var gauges = await _gaugeService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(gauges);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromForm] BackflowGaugeDto gauge, [Required] IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var added = await _gaugeService.AddAsync(professionalId, gauge, stream, file.FileName, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{gaugeId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int gaugeId, [FromBody] BackflowGaugeDto gauge, CancellationToken cancellationToken)
    {
        var updated = await _gaugeService.UpdateAsync(professionalId, gaugeId, gauge, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpGet("{gaugeId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        var url = await _gaugeService.GetFileUrlAsync(professionalId, gaugeId, cancellationToken);

        return url == null ? NotFound() : Ok(url);
    }

    [HttpDelete("{gaugeId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        await _gaugeService.DeleteAsync(professionalId, gaugeId, cancellationToken);

        return Ok();
    }
}
