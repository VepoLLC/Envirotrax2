using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class BackflowSettingsProfile : Profile
{
    public BackflowSettingsProfile()
    {
        CreateMap<BackflowSettings, BackflowSettingsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.WaterSupplierId));

        CreateMap<BackflowSettingsDto, BackflowSettings>()
            .ForMember(dest => dest.WaterSupplierId, opt => opt.MapFrom(src => src.Id));
    }
}
