using AutoMapper;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;

namespace Envirotrax.App.Server.Domain.Mapping.GisAreas;

public class GisAreaProfile : Profile
{
    public GisAreaProfile()
    {
        CreateMap<GisArea, GisAreaDto>().ReverseMap();

        CreateMap<GisArea, ReferencedGisAreaDto>().ReverseMap();
    }
}
