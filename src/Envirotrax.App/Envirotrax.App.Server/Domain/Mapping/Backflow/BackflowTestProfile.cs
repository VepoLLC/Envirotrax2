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
                dto.WaterSupplier ??= new() { Id = model.WaterSupplierId };

                if (model.SiteId.HasValue)
                {
                    dto.Site ??= new() { Id = model.SiteId.Value };
                }

                if (model.ProfessionalId.HasValue)
                {
                    dto.Professional ??= new() { Id = model.ProfessionalId.Value };
                }

                if (model.BpatId.HasValue)
                {
                    dto.Bpat ??= new() { Id = model.BpatId.Value };
                }

                if (model.BpatStateId.HasValue)
                {
                    dto.BpatState ??= new() { Id = model.BpatStateId.Value };
                }

                if (model.PropertyStateId.HasValue)
                {
                    dto.PropertyState ??= new() { Id = model.PropertyStateId.Value };
                }

                if (model.MailingStateId.HasValue)
                {
                    dto.MailingState ??= new() { Id = model.MailingStateId.Value };
                }

                if (model.ApprovedById.HasValue)
                {
                    dto.ApprovedBy ??= new() { Id = model.ApprovedById.Value };
                }

                if (model.RejectedById.HasValue)
                {
                    dto.RejectedBy ??= new() { Id = model.RejectedById.Value };
                }
            })
            .ReverseMap()
            .ForMember(m => m.WaterSupplier, opt => opt.Ignore())
            .ForMember(m => m.WaterSupplierId, opt => opt.MapFrom(dto => dto.WaterSupplier != null && dto.WaterSupplier.Id.HasValue ? dto.WaterSupplier.Id.Value : 0))
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site != null ? dto.Site.Id : (int?)null))
            .ForMember(m => m.Bpat, opt => opt.Ignore())
            .ForMember(m => m.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null ? dto.Professional.Id : (int?)null))
            .ForMember(m => m.BpatId, opt => opt.MapFrom(dto => dto.Bpat != null ? dto.Bpat.Id : (int?)null))
            .ForMember(m => m.BpatState, opt => opt.Ignore())
            .ForMember(m => m.BpatStateId, opt => opt.MapFrom(dto => dto.BpatState != null ? dto.BpatState.Id : (int?)null))
            .ForMember(m => m.PropertyState, opt => opt.Ignore())
            .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : (int?)null))
            .ForMember(m => m.MailingState, opt => opt.Ignore())
            .ForMember(m => m.MailingStateId, opt => opt.MapFrom(dto => dto.MailingState != null ? dto.MailingState.Id : (int?)null))
            .ForMember(m => m.ApprovedBy, opt => opt.Ignore())
            .ForMember(m => m.ApprovedById, opt => opt.MapFrom(dto => dto.ApprovedBy != null ? dto.ApprovedBy.Id : (int?)null))
            .ForMember(m => m.RejectedBy, opt => opt.Ignore())
            .ForMember(m => m.RejectedById, opt => opt.MapFrom(dto => dto.RejectedBy != null ? dto.RejectedBy.Id : (int?)null))
            .ForMember(m => m.UpdatedBy, opt => opt.Ignore())
            .ForMember(m => m.CreatedBy, opt => opt.Ignore())
            .ForMember(m => m.DeletedBy, opt => opt.Ignore());
    }
}
