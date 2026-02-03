
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalUserProfile : Profile
{
    public ProfessionalUserProfile()
    {
        CreateMap<ProfessionalUser, ProfessionalUserDto>()
            .ForMember(proUser => proUser.Id, opt => opt.MapFrom(proUser => proUser.UserId))
            .ReverseMap()
            .ForMember(proUser => proUser.User, opt => opt.Ignore())
            .ForMember(proUser => proUser.UserId, opt => opt.MapFrom(proUser => proUser.Id));
    }
}