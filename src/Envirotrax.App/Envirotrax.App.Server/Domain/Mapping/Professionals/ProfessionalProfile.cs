
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

                if (model.BillingStateId.HasValue)
                {
                    dto.BillingState ??= new()
                    {
                        Id = model.BillingStateId.Value
                    };
                }
            })
            .ReverseMap()
            .ForMember(pro => pro.State, opt => opt.Ignore())
            .ForMember(pro => pro.StateId, opt => opt.MapFrom(pro => pro.State!.Id))
            .ForMember(pro => pro.BillingState, opt => opt.Ignore())
            .ForMember(pro => pro.BillingStateId, opt => opt.MapFrom(pro => pro.BillingState!.Id))
            .ForMember(pro => pro.CreatedTime, opt => opt.Ignore());

        CreateMap<Professional, ReferencedProfessionalDto>();
    }
}