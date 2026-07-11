using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogTripTicketProfile : Profile
{
    public FogTripTicketProfile()
    {
        CreateMap<FogTripTicket, FogTripTicketDto>()
            .ForMember(dest => dest.WaterSupplier, opt => opt.Ignore())
            .ForMember(dest => dest.Site, opt => opt.Ignore())
            .ForMember(dest => dest.Professional, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyState, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.WaterSupplier ??= model.WaterSupplier != null
                    ? new ReferencedWaterSupplierDto { Id = model.WaterSupplierId, Name = model.WaterSupplier.Name }
                    : new ReferencedWaterSupplierDto { Id = model.WaterSupplierId };

                dto.Site ??= new ReferencedSiteDto { Id = model.SiteId };
                dto.Professional ??= new ReferencedProfessionalDto { Id = model.ProfessionalId };

                if (model.PropertyStateId.HasValue)
                {
                    dto.PropertyState ??= new ReferencedStateDto { Id = model.PropertyStateId.Value };
                }
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site != null ? dto.Site.Id : (int?)null))
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null ? dto.Professional.Id ?? 0 : 0))
            .ForMember(m => m.PropertyState, opt => opt.Ignore())
            .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : (int?)null))
            .ForMember(m => m.Vehicle, opt => opt.Ignore())
            .ForMember(m => m.ReceiverDisposalSite, opt => opt.Ignore())
            .ForMember(m => m.WaterSupplier, opt => opt.Ignore())
            .ForMember(m => m.CreatedBy, opt => opt.Ignore())
            .ForMember(m => m.UpdatedBy, opt => opt.Ignore())
            .ForMember(m => m.DeletedBy, opt => opt.Ignore());
    }
}
