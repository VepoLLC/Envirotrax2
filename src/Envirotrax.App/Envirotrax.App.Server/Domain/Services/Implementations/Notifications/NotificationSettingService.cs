using AutoMapper;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class NotificationSettingService : Service<NotificationSetting, NotificationSettingDto>, INotificationSettingService
{
    private readonly ITenantProvidersService _tenantProvider;

    public NotificationSettingService(
        IMapper mapper,
        INotificationSettingRepository repository,
        ITenantProvidersService tenantProvider)
        : base(mapper, repository)
    {
        _tenantProvider = tenantProvider;
    }

    public override Task<NotificationSettingDto> AddAsync(NotificationSettingDto dto)
    {
        SetLoggedInUserWhenNotSpecified(dto);

        return base.AddAsync(dto);
    }

    public override Task<NotificationSettingDto> UpdateAsync(NotificationSettingDto dto)
    {
        SetLoggedInUserWhenNotSpecified(dto);

        return base.UpdateAsync(dto);
    }

    private void SetLoggedInUserWhenNotSpecified(NotificationSettingDto dto)
    {
        dto.UserId ??= _tenantProvider.UserId;
    }
}
