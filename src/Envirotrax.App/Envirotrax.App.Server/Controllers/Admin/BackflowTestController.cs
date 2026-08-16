using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/tests")]
public class BackflowTestController : AdminBaseController
{
    private readonly IBackflowTestService _testService;

    public BackflowTestController(IBackflowTestService testService)
    {
        _testService = testService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var tests = await _testService.SearchAsync(pageInfo, query, paymentStatus, null, cancellationToken);

        return Ok(tests);
    }
}
