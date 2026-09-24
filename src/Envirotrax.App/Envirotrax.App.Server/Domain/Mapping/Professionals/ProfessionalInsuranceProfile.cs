
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

        // Sorting and filtering are resolved against the entity, so every flat column name on the grid
        // needs a member path here. ContactName and ProfessionalType have none and stay unsortable.
        CreateMap<ProfessionalInsurance, WaterSupplierInsuranceDto>()
            .ForMember(dto => dto.SubmittedOn, opt => opt.MapFrom(i => i.CreatedTime))
            .ForMember(dto => dto.UserEmail, opt => opt.MapFrom(i => i.CreatedBy!.Email))
            .ForMember(dto => dto.CompanyName, opt => opt.MapFrom(i => i.Professional!.Name))
            .ForMember(dto => dto.ContactName, opt => opt.Ignore())
            .ForMember(dto => dto.ProfessionalType, opt => opt.Ignore());
    }
}