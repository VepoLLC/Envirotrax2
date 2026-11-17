using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Logs;

[Route("api/record-logs")]
public class RecordLogController : AdminBaseController
{
    private readonly IRecordLogService _recordLogService;

    public RecordLogController(IRecordLogService recordLogService)
    {
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] RecordLogSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(logs);
    }
}
