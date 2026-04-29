using AutoMapper;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

namespace Envirotrax.App.Server.Domain.Mapping.GisAreas;

public class GisAreaCoordinateProfile : Profile
{
    public GisAreaCoordinateProfile()
    {
        CreateMap<GisAreaCoordinate, GisAreaCoordinateDto>()
            .AfterMap((model, dto) =>
            {
                dto.Area ??= new ReferencedGisAreaDto { Id = model.AreaId };
            })
            .ReverseMap()
            .ForMember(dest => dest.Area, opt => opt.Ignore())
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.Area.Id));
    }
}
