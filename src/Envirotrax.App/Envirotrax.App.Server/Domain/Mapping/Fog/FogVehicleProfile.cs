using AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Domain.Mapping.Fog;

public class FogVehicleProfile : Profile
{
    public FogVehicleProfile()
    {
        CreateMap<FogVehicle, FogVehicleDto>()
            .ForMember(dto => dto.Professional, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Professional ??= new()
                {
                    Id = model.ProfessionalId
                };
            })
            .ReverseMap()
            .ForMember(v => v.Professional, opt => opt.Ignore())
            .ForMember(v => v.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null ? dto.Professional.Id : (int?)null))
            .ForMember(v => v.CreatedBy, opt => opt.Ignore())
            .ForMember(v => v.DeletedBy, opt => opt.Ignore());
    }
}
