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

            CreateMap<Data.Models.States.State, ReferencedStateDto>()
                .ReverseMap()
                .ForMember(s => s.Name, opt => opt.Ignore());
        }
    }
}
