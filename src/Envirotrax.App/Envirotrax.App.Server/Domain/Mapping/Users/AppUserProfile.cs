using AutoMapper;

namespace Envirotrax.App.Server.Domain.Mapping.Users
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            CreateMap<Data.Models.Users.AppUser, Domain.DataTransferObjects.Users.AppUserDto>()
                .ReverseMap();
        }
    }
}
