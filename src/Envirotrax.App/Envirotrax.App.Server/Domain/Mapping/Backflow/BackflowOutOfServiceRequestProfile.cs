using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Mapping.Backflow;

public class BackflowOutOfServiceRequestProfile : Profile
{
    public BackflowOutOfServiceRequestProfile()
    {
        CreateMap<BackflowOutOfServiceRequest, BackflowOutOfServiceRequestDto>()
            .ForMember(dto => dto.WaterSupplier, opt => opt.Ignore())
            .ForMember(dto => dto.Professional, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.WaterSupplier ??= new()
                {
                    Id = model.WaterSupplierId
                };

                dto.Professional ??= new()
                {
                    Id = model.ProfessionalId
                };
            })
            .ReverseMap()
            .ForMember(m => m.WaterSupplier, opt => opt.Ignore())
            .ForMember(m => m.WaterSupplierId, opt => opt.MapFrom(dto => dto.WaterSupplier != null && dto.WaterSupplier.Id.HasValue ? dto.WaterSupplier.Id.Value : 0))
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null && dto.Professional.Id.HasValue ? dto.Professional.Id.Value : 0))
            .ForMember(m => m.Bpat, opt => opt.Ignore())
            .ForMember(m => m.Test, opt => opt.Ignore())
            .ForMember(m => m.ReplacementAssemblyTest, opt => opt.Ignore());
    }
}
