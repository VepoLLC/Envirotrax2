
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalSupplierProfile : Profile
{
    public ProfessionalSupplierProfile()
    {
        CreateMap<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>()
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
            .ForMember(model => model.Professional, opt => opt.Ignore())
            .ForMember(model => model.WaterSupplier, opt => opt.Ignore())
            .ForMember(model => model.WaterSupplierId, opt => opt.MapFrom(dto => dto.WaterSupplier.Id));

        CreateMap<AvailableWaterSupplier, AvailableWaterSupplierDto>();
    }
}