
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class ProfessionalLicenseTypeProfile : Profile
{
    public ProfessionalLicenseTypeProfile()
    {
        CreateMap<ProfessionalLicenseType, ProfessionalLicenseTypeDto>()
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
            .ForMember(type => type.State, opt => opt.Ignore())
            .ForMember(type => type.StateId, opt => opt.MapFrom(type => type.State!.Id));

        CreateMap<ProfessionalLicenseType, ReferencedProfessionalLicenseTypeDto>()
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
            .ForMember(type => type.State, opt => opt.Ignore())
            .ForMember(type => type.StateId, opt => opt.MapFrom(type => type.State!.Id));
    }
}