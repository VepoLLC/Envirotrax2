using AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.Sites;

public class SiteProfile : Profile
{
    public SiteProfile()
    {
        CreateMap<Site, SiteDto>()
            .AfterMap((model, dto) =>
            {
                dto.WaterSupplier ??= new ReferencedWaterSupplierDto
                {
                    Id = model.WaterSupplierId
                };
                dto.State ??= model.StateId.HasValue ? new ReferencedStateDto { Id = model.StateId } : null;
                dto.MailingState ??= model.MailingStateId.HasValue ? new ReferencedStateDto { Id = model.MailingStateId } : null;
            })
            .ReverseMap()
            .ForMember(s => s.WaterSupplier, opt => opt.Ignore())
            .ForMember(s => s.WaterSupplierId, opt => opt.MapFrom(dto => dto.WaterSupplier!.Id))
            .ForMember(s => s.State, opt => opt.Ignore())
            .ForMember(s => s.StateId, opt => opt.MapFrom(dto => dto.State != null ? dto.State.Id : (int?)null))
            .ForMember(s => s.MailingState, opt => opt.Ignore())
            .ForMember(s => s.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : (int?)null));
    }
}
