using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogDisposalSiteProfile : Profile
{
    public FogDisposalSiteProfile()
    {
        CreateMap<FogDisposalSite, FogDisposalSiteDto>()
            .ForMember(dto => dto.State, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                if (model.StateId.HasValue)
                {
                    dto.State ??= new() { Id = model.StateId.Value };
                }
            })
            .ReverseMap()
            .ForMember(m => m.State, opt => opt.Ignore())
            .ForMember(m => m.StateId, opt => opt.MapFrom(dto => dto.State != null ? dto.State.Id : (int?)null))
            .ForMember(m => m.CreatedBy, opt => opt.Ignore())
            .ForMember(m => m.DeletedBy, opt => opt.Ignore());
    }
}
