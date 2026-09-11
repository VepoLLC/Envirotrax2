using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/replacements")]
public class BackflowReplacementController : AdminBaseController
{
    private readonly IBackflowReplacementService _replacementService;

    public BackflowReplacementController(IBackflowReplacementService replacementService)
    {
        _replacementService = replacementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] bool onHold, CancellationToken cancellationToken)
    {
        var replacements = await _replacementService.GetAllAsync(pageInfo, query, onHold, cancellationToken);

        return Ok(replacements);
    }

    [HttpGet("{id}/replaced-assembly")]
    public async Task<IActionResult> GetReplacedAssemblyAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _replacementService.GetReplacedAssemblyAsync(id, cancellationToken);

        if (test == null)
        {
            return NoContent();
        }

        return Ok(test);
    }

    [HttpPut("{id}/hold")]
    public async Task<IActionResult> UpdateHoldAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool onHold, CancellationToken cancellationToken)
    {
        var replacement = await _replacementService.UpdateHoldAsync(id, waterSupplierId, onHold, cancellationToken);

        if (replacement == null)
        {
            return NotFound();
        }

        return Ok(replacement);
    }

    [HttpPut("{id}/cleared")]
    public async Task<IActionResult> UpdateClearedAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool cleared, CancellationToken cancellationToken)
    {
        var replacement = await _replacementService.UpdateClearedAsync(id, waterSupplierId, cleared, cancellationToken);

        if (replacement == null)
        {
            return NotFound();
        }

        return Ok(replacement);
    }
}
