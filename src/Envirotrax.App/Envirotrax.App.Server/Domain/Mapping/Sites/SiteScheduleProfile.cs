using AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Mapping.Sites;

public class SiteScheduleProfile : Profile
{
    public SiteScheduleProfile()
    {
        CreateMap<SiteSchedule, SiteScheduleDto>()
            .ReverseMap()
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.User, opt => opt.Ignore())
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.CreatedBy, opt => opt.Ignore());
    }
}
