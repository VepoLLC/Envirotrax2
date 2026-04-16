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

                if (model.BpatStateId.HasValue)
                    dto.BpatState ??= new() { Id = model.BpatStateId.Value };

                if (model.PropertyStateId.HasValue)
                    dto.PropertyState ??= new() { Id = model.PropertyStateId.Value };

                if (model.MailingStateId.HasValue)
                    dto.MailingState ??= new() { Id = model.MailingStateId.Value };
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site != null ? dto.Site.Id : (int?)null))
            .ForMember(m => m.Bpat, opt => opt.Ignore())
            .ForMember(m => m.ApprovedBy, opt => opt.Ignore())
            .ForMember(m => m.RejectedBy, opt => opt.Ignore())
            .ForMember(m => m.BpatState, opt => opt.Ignore())
            .ForMember(m => m.BpatStateId, opt => opt.MapFrom(dto => dto.BpatState != null ? dto.BpatState.Id : null))
            .ForMember(m => m.PropertyState, opt => opt.Ignore())
            .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : null))
            .ForMember(m => m.MailingState, opt => opt.Ignore())
            .ForMember(m => m.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : null));
    }
}
