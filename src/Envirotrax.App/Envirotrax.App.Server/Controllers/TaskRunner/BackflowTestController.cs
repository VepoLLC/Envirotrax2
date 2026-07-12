
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/backflow-tests")]
public class BackflowTestController : TaskRunnerBaseContoller
{
    private readonly IBackflowTestService _backflowTestService;

    public BackflowTestController(IBackflowTestService backflowTestService)
    {
        _backflowTestService = backflowTestService;
    }

    [HttpGet("renewal/pending")]
    public async Task<IActionResult> GetAllPendingRenewalAsync([FromQuery] int batchSize, CancellationToken cancellationToken)
    {
        var tests = await _backflowTestService.GetAllPendingRenewalAsync(batchSize, cancellationToken);
        return Ok(tests);
    }

    [HttpPost("{testId}/extend-date")]
    public async Task<IActionResult> ExtendDateAsync(int testId, CancellationToken cancellationToken)
    {
        var test = await _backflowTestService.ExtendDateAsync(testId, cancellationToken);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }
}
