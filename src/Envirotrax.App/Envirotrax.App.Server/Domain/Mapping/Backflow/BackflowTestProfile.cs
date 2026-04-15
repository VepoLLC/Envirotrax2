using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Mapping.Backflow;

public class BackflowTestProfile : Profile
{
    public BackflowTestProfile()
    {
        CreateMap<BackflowTest, BackflowTestDto>()
            .AfterMap((model, dto) =>
            {
                dto.Site ??= model.SiteId.HasValue ? new() { Id = model.SiteId.Value } : null;
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site != null ? dto.Site.Id : (int?)null));
    }
}
