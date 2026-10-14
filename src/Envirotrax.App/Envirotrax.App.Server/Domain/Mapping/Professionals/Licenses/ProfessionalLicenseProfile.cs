using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals.Licenses;

public class ProfessionalLicenseProfile : Profile
{
    public ProfessionalLicenseProfile()
    {
        CreateMap<ProfessionalLicense, ProfessionalLicenseDto>()
            .AfterMap((model, dto) =>
            {
                dto.LicenseType ??= new()
                {
                    Id = model.LicenseTypeId
                };
            })
            .ReverseMap()
            .ForMember(l => l.Professional, opt => opt.Ignore())
            .ForMember(l => l.CreatedBy, opt => opt.Ignore())
            .ForMember(l => l.LicenseType, opt => opt.Ignore())
            .ForMember(l => l.LicenseTypeId, opt => opt.MapFrom(l => l.LicenseType.Id));
    }
}
