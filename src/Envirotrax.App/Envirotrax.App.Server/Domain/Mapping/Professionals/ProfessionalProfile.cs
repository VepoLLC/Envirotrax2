
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalProfile : Profile
{
    public ProfessionalProfile()
    {
        CreateMap<Professional, ProfessionalDto>()
            .AfterMap((model, dto) =>
            {
                if (model.StateId.HasValue)
                {
                    dto.State ??= new()
                    {
                        Id = model.StateId.Value
                    };
                }
            })
            .ReverseMap()
            .ForMember(pro => pro.State, opt => opt.Ignore())
            .ForMember(pro => pro.StateId, opt => opt.MapFrom(pro => pro.State!.Id));

        CreateMap<Professional, ReferencedProfessionalDto>();
    }
}