
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/backflow-tests")]
public class BackflowTestController : TaskRunnerBaseContoller
{
    private readonly IBackflowTestService _backflowTestService;
    private readonly ISiteService _siteService;

    public BackflowTestController(IBackflowTestService backflowTestService, ISiteService siteService)
    {
        _backflowTestService = backflowTestService;
        _siteService = siteService;
    }

    [HttpGet("renewal/pending-sites")]
    public async Task<IActionResult> GetAllPendingSitesRenewalAsync([FromQuery] int batchSize, CancellationToken cancellationToken)
    {
        var sites = await _siteService.GetAllPendingRenewalAsync(batchSize, cancellationToken);
        return Ok(sites);
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
