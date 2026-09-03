using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/tests")]
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
        var tests = await _testService.SearchAsync(pageInfo, query, paymentStatus, cancellationToken);

        return Ok(tests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testService.GetAsync(id, cancellationToken);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromQuery] int waterSupplierId, [FromBody] BackflowTestUpdateRequest request, CancellationToken cancellationToken)
    {
        var test = await _testService.UpdateAsync(id, waterSupplierId, request, cancellationToken);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpPost("{id}/images/{imageType}")]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromQuery] int waterSupplierId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();

        var test = await _testService.UploadImageAsync(id, waterSupplierId, imageType, stream, file.FileName, cancellationToken);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }

    [HttpGet("{id}/counts")]
    public async Task<IActionResult> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        var counts = await _testService.GetCountsAsync(id, cancellationToken);

        return Ok(counts);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _testService.GetLogsAsync(id, cancellationToken);

        return Ok(logs);
    }
}
