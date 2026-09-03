using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/tests")]
public class BackflowTestController : AdminBaseController
{
    private readonly IBackflowTestService _testService;
    private readonly IRecordLogService _recordLogService;

    public BackflowTestController(IBackflowTestService testService, IRecordLogService recordLogService)
    {
        _testService = testService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var tests = await _testService.SearchAsync(pageInfo, query, paymentStatus, cancellationToken);

        return Ok(tests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testService.GetForAdminAsync(id, cancellationToken);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] BackflowTestAdminUpdateRequest request)
    {
        var test = await _testService.UpdateForAdminAsync(id, request);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpPost("{id}/images/{imageType}")]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();

        var updated = await _testService.UpdateImageAsync(id, imageType, stream, file.FileName);

        if (updated == null)
        {
            return NotFound();
        }

        var test = await _testService.GetForAdminAsync(id, CancellationToken.None);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpGet("{id}/counts")]
    public async Task<IActionResult> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        var recordLogCount = await _recordLogService.GetCountByRecordAsync(RecordLogTableNames.BackflowTests, id, cancellationToken);

        return Ok(new BackflowTestCountsDto
        {
            RecordLogCount = recordLogCount
        });
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.BackflowTests, id, cancellationToken);

        return Ok(logs);
    }
}
