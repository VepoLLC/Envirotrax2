
using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class WaterSupplierProfile : Profile
{
    public WaterSupplierProfile()
    {
        CreateMap<WaterSupplier, WaterSupplierDto>()
            .AfterMap((model, dto) =>
            {
                if (model.ParentId.HasValue)
                {
                    dto.Parent ??= new ReferencedWaterSupplierDto
                    {
                        Id = model.ParentId.Value
                    };
                }
            })
            .ReverseMap()
            .ForMember(supplier => supplier.Parent, opt => opt.Ignore())
            .ForMember(supplier => supplier.ParentId, opt => opt.MapFrom(supplier => supplier.Parent!.Id));

        CreateMap<WaterSupplier, ReferencedWaterSupplierDto>()
            .ReverseMap();
    }
}