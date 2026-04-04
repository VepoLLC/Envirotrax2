
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalInsuranceProfile : Profile
{
    public ProfessionalInsuranceProfile()
    {
        CreateMap<ProfessionalInsurance, ProfessionalInsuranceDto>()
            .AfterMap((model, dto) =>
            {
                dto.Professional ??= new()
                {
                    Id = model.ProfessionalId
                };
            }).ReverseMap()
            .ForMember(i => i.Professional, opt => opt.Ignore())
            .ForMember(i => i.ProfessionalId, opt => opt.MapFrom(i => i.Professional!.Id));
    }
}