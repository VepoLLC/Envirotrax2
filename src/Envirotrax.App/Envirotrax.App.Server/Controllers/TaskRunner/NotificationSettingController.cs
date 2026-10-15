using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/notification-settings")]
public class NotificationSettingController : TaskRunnerBaseContoller
{
    private readonly INotificationDigestService _digestService;

    public NotificationSettingController(INotificationDigestService digestService)
    {
        _digestService = digestService;
    }

    [HttpGet("digests/pending")]
    public async Task<IActionResult> GetPendingDigestsAsync([FromQuery] int batchSize, CancellationToken cancellationToken)
    {
        var settings = await _digestService.GetPendingAsync(batchSize, cancellationToken);
        return Ok(settings);
    }

    [HttpPost("{id}/schedule-digest")]
    public async Task<IActionResult> ScheduleDigestAsync(int id, CancellationToken cancellationToken)
    {
        await _digestService.ScheduleAsync(id, cancellationToken);
        return NoContent();
    }
}
