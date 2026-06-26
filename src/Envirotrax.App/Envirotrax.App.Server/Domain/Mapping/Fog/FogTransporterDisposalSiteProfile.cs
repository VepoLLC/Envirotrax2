using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogTransporterDisposalSiteProfile : Profile
{
    public FogTransporterDisposalSiteProfile()
    {
        CreateMap<FogTransporterDisposalSite, FogTransporterDisposalSiteDto>()
            .ForMember(dto => dto.Professional, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Professional ??= new() { Id = model.ProfessionalId };
            })
            .ReverseMap()
            .ForMember(m => m.DisposalSite, opt => opt.Ignore())
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null ? dto.Professional.Id : (int?)null))
            .ForMember(m => m.CreatedBy, opt => opt.Ignore());
    }
}
