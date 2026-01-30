using AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Mapping.Sites;

public class SiteProfile : Profile
{
    public SiteProfile()
    {
        CreateMap<Site, SiteDto>()
            .ReverseMap();
    }
}
