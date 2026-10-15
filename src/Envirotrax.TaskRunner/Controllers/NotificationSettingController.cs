
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Envirotrax.TaskRunner.Controllers;

[ApiController]
[Route("api/notification-settings")]
public class NotificationSettingController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly IInternalApiClientService _internalApi;
    private readonly NotificationDigestOptions _options;

    public NotificationSettingController(
        IQueueService queueService,
        IInternalApiClientService internalApi,
        IOptions<NotificationDigestOptions> options)
    {
        _queueService = queueService;
        _internalApi = internalApi;
        _options = options.Value;
    }

    [HttpPost("schedule-digests")]
    public async Task<IActionResult> ScheduleDigestsAsync(CancellationToken cancellationToken)
    {
        var asOfTime = DateTimeOffset.UtcNow;

        var settings = await _internalApi.GetAsync<ICollection<NotificationSettingDto>>(
            null,
            $"api/task-runner/notification-settings/digests/pending?batchSize={_options.BatchSize}&asOfTime={Uri.EscapeDataString(asOfTime.ToString("O"))}",
            cancellationToken);

        await _queueService.EnsureQueueExistsAsync(QueueNames.NotificationSettings.ScheduleDigest, cancellationToken);

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken };

        await Parallel.ForEachAsync(settings!, parallelOptions, async (setting, ct) =>
        {
            if (!ct.IsCancellationRequested)
            {
                setting.AsOfTime = asOfTime;

                await _queueService.SendMessageAsync(QueueNames.NotificationSettings.ScheduleDigest, setting, ct);
            }
        });

        return Accepted(settings!.Count);
    }
}
