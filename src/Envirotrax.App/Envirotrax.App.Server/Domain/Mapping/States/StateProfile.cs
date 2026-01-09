using AutoMapper;

namespace Envirotrax.App.Server.Domain.Mapping.States
{
    public class StateProfile : Profile
    {
        public StateProfile()
        {
            CreateMap<Data.Models.States.State, DataTransferObjects.States.StateDto>()
                .ReverseMap();
        }
    }
}
