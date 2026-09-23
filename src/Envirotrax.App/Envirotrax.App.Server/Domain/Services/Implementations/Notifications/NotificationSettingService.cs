using AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class NotificationSettingService : Service<NotificationSetting, NotificationSettingDto>, INotificationSettingService
{
    private readonly INotificationSettingRepository _repository;
    private readonly ITenantProvidersService _tenantProvider;
    private readonly IRecordLogService _recordLogService;

    public NotificationSettingService(
        IMapper mapper,
        INotificationSettingRepository repository,
        ITenantProvidersService tenantProvider,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _repository = repository;
        _tenantProvider = tenantProvider;
        _recordLogService = recordLogService;
    }

    public Task<List<NotificationSetting>> GetCandidateSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return _repository.GetCandidateSettingsAsync(waterSupplierId, cancellationToken);
    }

    public override Task<NotificationSettingDto> AddAsync(NotificationSettingDto dto)
    {
        SetLoggedInUserWhenNotSpecified(dto);

        return base.AddAsync(dto);
    }

    public override async Task<NotificationSettingDto> UpdateAsync(NotificationSettingDto dto)
    {
        SetLoggedInUserWhenNotSpecified(dto);

        var model = MapToModel(dto)!;
        var saved = await _repository.UpdateSettingAsync(model);

        if (saved == null)
        {
            throw new InvalidOperationException($"Notification setting {dto.Id} not found.");
        }

        return MapToDto(saved)!;
    }

    public override async Task<NotificationSettingDto?> DeleteAsync(int id)
    {
        var deleted = await base.DeleteAsync(id);

        if (deleted != null)
        {
            // recordLog manual
            await _recordLogService.AddAsync(RecordLogTableNames.NotificationSettings, deleted.Id, _tenantProvider.WaterSupplierId, RecordLogType.Delete,
                $"Deleted notification setting — Description: '{deleted.Description}'");
        }

        return deleted;
    }

    private void SetLoggedInUserWhenNotSpecified(NotificationSettingDto dto)
    {
        dto.UserId ??= _tenantProvider.UserId;
    }
}
