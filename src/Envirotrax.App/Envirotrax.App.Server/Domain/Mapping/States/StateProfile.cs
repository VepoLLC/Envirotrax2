using AutoMapper;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.Mapping.States
{
    public class StateProfile : Profile
    {
        public StateProfile()
        {
            CreateMap<Data.Models.States.State, StateDto>()
                .ReverseMap();
        }
    }
}
