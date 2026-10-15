
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Workers.Notifications;

public class NotificationDigestWorker : IQueueWorker<NotificationSettingDto>
{
    private readonly IInternalApiClientService _internalApi;

    public NotificationDigestWorker(IInternalApiClientService internalApi)
    {
        _internalApi = internalApi;
    }

    public async Task DoWorkAsync(NotificationSettingDto? setting, CancellationToken cancellationToken)
    {
        var apiRequest = new ServiceMessageDto<NotificationSettingDto>(waterSupplierId: setting!.WaterSupplier.Id, loggedInUserId: null)
        {
            Data = setting
        };

        await _internalApi.PostAsync<NotificationSettingDto, object>($"/api/task-runner/notification-settings/{setting.Id}/schedule-digest", apiRequest, cancellationToken);
    }
}
