using AutoMapper;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

namespace Envirotrax.App.Server.Domain.Mapping.Notifications;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ReverseMap()
            .ForMember(notification => notification.User, opt => opt.Ignore())
            .ForMember(notification => notification.ParentWaterSupplier, opt => opt.Ignore())
            .ForMember(notification => notification.CreatedBy, opt => opt.Ignore());
    }
}
