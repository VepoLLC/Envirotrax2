using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogVehiclePermitProfile : Profile
{
    public FogVehiclePermitProfile()
    {
        CreateMap<FogVehiclePermit, FogVehiclePermitDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(permit => permit.VehicleId))
            .ReverseMap()
            .ForMember(p => p.VehicleId, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(p => p.CreatedTime, opt => opt.Ignore())
            .ForMember(p => p.Vehicle, opt => opt.Ignore())
            .ForMember(p => p.WaterSupplier, opt => opt.Ignore())
            .ForMember(p => p.CreatedBy, opt => opt.Ignore());

        CreateMap<FogVehicle, FogVehiclePermitSearchDto>();
    }
}
