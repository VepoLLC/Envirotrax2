using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogInspectionProfile : Profile
{
    public FogInspectionProfile()
    {
        CreateMap<FogInspection, FogInspectionDto>()
            .ForMember(dest => dest.Site, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyState, opt => opt.Ignore())
            .ForMember(dest => dest.MailingState, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Site ??= new ReferencedSiteDto { Id = model.SiteId };

                if (model.PropertyStateId.HasValue)
                {
                    dto.PropertyState ??= new() { Id = model.PropertyStateId.Value };
                }

                if (model.MailingStateId.HasValue)
                {
                    dto.MailingState ??= new() { Id = model.MailingStateId.Value };
                }
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site!.Id))
            .ForMember(m => m.PropertyState, opt => opt.Ignore())
            .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : null))
            .ForMember(m => m.MailingState, opt => opt.Ignore())
            .ForMember(m => m.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : null))
            .ForMember(m => m.CreatedBy, opt => opt.Ignore())
            .ForMember(m => m.UpdatedBy, opt => opt.Ignore())
            .ForMember(m => m.DeletedBy, opt => opt.Ignore());
    }
}
