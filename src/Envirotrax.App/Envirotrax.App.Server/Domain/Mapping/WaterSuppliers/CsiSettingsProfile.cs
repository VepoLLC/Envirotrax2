using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class CsiSettingsProfile : Profile
{
    public CsiSettingsProfile()
    {
        CreateMap<CsiSettings, CsiSettingsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WaterSupplierId));

        CreateMap<CsiSettingsDto, CsiSettings>()
            .ForMember(dest => dest.WaterSupplierId, opt => opt.MapFrom(src => src.Id));
    }
}
