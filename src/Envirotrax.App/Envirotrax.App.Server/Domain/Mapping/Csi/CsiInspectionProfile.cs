using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectionProfile : Profile
{
    public CsiInspectionProfile()
    {
        CreateMap<CsiInspection, CsiInspectionDto>()
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
            .ForMember(m => m.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : null));
    }
}
