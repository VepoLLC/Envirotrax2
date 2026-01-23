using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class GeneralSettingsProfile : Profile
{
    public GeneralSettingsProfile()
    {
        CreateMap<GeneralSettings, GeneralSettingsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WaterSupplierId));

        CreateMap<GeneralSettingsDto, GeneralSettings>()
            .ForMember(dest => dest.WaterSupplierId, opt => opt.MapFrom(src => src.Id));
    }
}
