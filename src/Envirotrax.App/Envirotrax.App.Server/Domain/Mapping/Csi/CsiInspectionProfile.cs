using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectionProfile : Profile
{
    public CsiInspectionProfile()
    {
        CreateMap<CsiInspection, CsiInspectionDto>()
            .ForMember(dto => dto.Site, opt => opt.Ignore())
            .ForMember(dto => dto.Professional, opt => opt.Ignore())
            .ForMember(dto => dto.InspectorUser, opt => opt.MapFrom(model => model.Inspector))
            .AfterMap((model, dto, context) =>
            {
                dto.Site ??= new ReferencedSiteDto
                {
                    Id = model.SiteId,
                    BusinessName = model.Site?.BusinessName,
                    AccountNumber = model.Site?.AccountNumber,
                };

                dto.Professional ??= new ReferencedProfessionalDto { Id = model.ProfessionalId };

                dto.WaterSupplier ??= new ReferencedWaterSupplierDto { Id = model.WaterSupplierId };

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
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.ProfessionalId, opt => opt.Ignore())
            .ForMember(m => m.Inspector, opt => opt.Ignore())
            .ForMember(m => m.InspectorId, opt => opt.Ignore())
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site!.Id))
            .ForMember(m => m.PropertyState, opt => opt.Ignore())
            .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : null))
            .ForMember(m => m.MailingState, opt => opt.Ignore())
            .ForMember(m => m.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : null));
    }
}
