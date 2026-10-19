using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/testers/{professionalId}/gauges")]
public class BackflowTesterGaugeController : AdminBaseController
{
    private readonly IBackflowGaugeService _gaugeService;

    public BackflowTesterGaugeController(IBackflowGaugeService gaugeService)
    {
        _gaugeService = gaugeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var gauges = await _gaugeService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(gauges);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromForm] CreateBackflowGaugeDto gauge)
    {
        // The route owns the professional, so never trust the one posted in the form.
        gauge.Professional = new ReferencedProfessionalDto
        {
            Id = professionalId
        };

        await using var stream = gauge.File.OpenReadStream();

        var added = await _gaugeService.AddWithFileAsync(stream, gauge.File.FileName, gauge);

        return Ok(added);
    }

    [HttpPut("{gaugeId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int gaugeId, [FromBody] BackflowGaugeDto gauge, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, gaugeId, cancellationToken))
        {
            return NotFound();
        }

        gauge.Id = gaugeId;

        var updated = await _gaugeService.UpdateAsync(gauge);

        return Ok(updated);
    }

    [HttpGet("{gaugeId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, gaugeId, cancellationToken))
        {
            return NotFound();
        }

        var url = await _gaugeService.GenerateFileUrlAsync(gaugeId, cancellationToken);

        return url == null ? NotFound() : Ok(url);
    }

    [HttpDelete("{gaugeId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, gaugeId, cancellationToken))
        {
            return NotFound();
        }

        var deleted = await _gaugeService.DeleteAsync(gaugeId);

        return deleted == null ? NotFound() : Ok();
    }

    // The gauge service keys every write on the gauge id alone, so the professional in the route has to be
    // confirmed as the owner before the record is touched.
    private async Task<bool> BelongsToProfessionalAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        var query = new Query();
        query.Filter.Add(new() { ColumnName = nameof(BackflowGaugeDto.Id), Value = gaugeId.ToString() });

        var owned = await _gaugeService.GetAllByProfessionalAsync(professionalId, new PageInfo(), query, cancellationToken);

        return owned.Data.Any();
    }
}
