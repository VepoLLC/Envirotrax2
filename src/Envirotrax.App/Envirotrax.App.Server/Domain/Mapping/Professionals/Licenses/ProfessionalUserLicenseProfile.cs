
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals.Licenses;

public class ProfessionalUserLicenseProfile : Profile
{
    public ProfessionalUserLicenseProfile()
    {
        CreateMap<ProfessionalUserLicense, ProfessionalUserLicenseDto>()
            .AfterMap((model, dto) =>
            {
                dto.User ??= new()
                {
                    Id = model.UserId
                };

                dto.LicenseType ??= new()
                {
                    Id = model.LicenseTypeId
                };
            })
            .ReverseMap()
            .ForMember(l => l.Professional, opt => opt.Ignore())
            .ForMember(l => l.User, opt => opt.Ignore())
            .ForMember(l => l.UserId, opt => opt.MapFrom(l => l.User.Id))
            .ForMember(l => l.CreatedBy, opt => opt.Ignore())
            .ForMember(l => l.LicenseType, opt => opt.Ignore())
            .ForMember(l => l.LicenseTypeId, opt => opt.MapFrom(l => l.LicenseType.Id));
    }
}
