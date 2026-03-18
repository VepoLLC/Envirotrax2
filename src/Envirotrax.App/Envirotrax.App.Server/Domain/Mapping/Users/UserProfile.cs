

using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Mapping.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, AppUserDto>()
            .ReverseMap();

        CreateMap<AppUser, ReferencedAppUserDto>()
            .ReverseMap();
    }
}