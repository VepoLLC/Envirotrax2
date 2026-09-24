using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/replacements")]
public class BackflowReplacementController : AdminBaseController
{
    private readonly IBackflowTestService _testService;

    public BackflowReplacementController(IBackflowTestService testService)
    {
        _testService = testService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] bool onHold, CancellationToken cancellationToken)
    {
        var replacements = await _testService.GetReplacementsAsync(pageInfo, query, onHold, cancellationToken);

        return Ok(replacements);
    }

    [HttpGet("{id}/replaced-assembly")]
    public async Task<IActionResult> GetReplacedAssemblyAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testService.GetReplacedAssemblyAsync(id, cancellationToken);

        if (test == null)
        {
            return NoContent();
        }

        return Ok(test);
    }

    [HttpPut("{id}/hold")]
    public async Task<IActionResult> UpdateHoldAsync(int id, [FromBody] bool onHold)
    {
        var replacement = await _testService.UpdateReplacementHoldAsync(id, onHold);

        if (replacement == null)
        {
            return NotFound();
        }

        return Ok(replacement);
    }

    [HttpPut("{id}/cleared")]
    public async Task<IActionResult> UpdateClearedAsync(int id, [FromBody] bool cleared)
    {
        var replacement = await _testService.UpdateReplacementClearedAsync(id, cleared);

        if (replacement == null)
        {
            return NotFound();
        }

        return Ok(replacement);
    }
}
