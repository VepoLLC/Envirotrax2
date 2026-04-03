
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Insurances;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals.Insurances;

public class ProfessionalInsuranceProfile : Profile
{
    public ProfessionalInsuranceProfile()
    {
        CreateMap<ProfessionalInsurance, ProfessionalInsurance>()
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