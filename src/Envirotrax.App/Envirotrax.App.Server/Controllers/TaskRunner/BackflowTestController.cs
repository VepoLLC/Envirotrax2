
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

    [HttpGet("renewal/pending-tests")]
    public async Task<IActionResult> GetAllPendingTestsRenewalAsync([FromQuery] int batchSize, CancellationToken cancellationToken)
    {
        var tests = await _backflowTestService.GetAllPendingTestsForRenewalAsync(batchSize, cancellationToken);
        return Ok(tests);
    }

    [HttpPost("sites/{siteId}/process-renewal")]
    public async Task<IActionResult> ProcessSiteRenewalAsync(int siteId, CancellationToken cancellationToken)
    {
        await _backflowTestService.ProcessSiteRenewalAsync(siteId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{testId}/process-test-renewal")]
    public async Task<IActionResult> ProcessTestRenewalAsync(int testId, CancellationToken cancellationToken)
    {
        await _backflowTestService.ProcessTestRenewalAsync(testId, cancellationToken);
        return NoContent();
    }
}
