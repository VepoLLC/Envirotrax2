using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Mapping.Notifications;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dto => dto.Test, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(notification => notification.User, opt => opt.Ignore())
            .ForMember(notification => notification.CreatedBy, opt => opt.Ignore());

        CreateMap<BackflowTest, NotificationTestDto>();
    }
}
