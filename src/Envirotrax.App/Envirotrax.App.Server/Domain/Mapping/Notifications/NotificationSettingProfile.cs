using AutoMapper;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Mapping.Notifications;

public class NotificationSettingProfile : Profile
{
    public NotificationSettingProfile()
    {
        CreateMap<NotificationSetting, NotificationSettingDto>()
            .ReverseMap()
            .ForMember(setting => setting.User, opt => opt.Ignore())
            .ForMember(setting => setting.CreatedBy, opt => opt.Ignore());
    }
}
