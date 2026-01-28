
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalProfile : Profile
{
    public ProfessionalProfile()
    {
        CreateMap<Professional, ProfessionalDto>()
            .ReverseMap();

        CreateMap<Professional, ReferencedProfessionalDto>();
    }
}