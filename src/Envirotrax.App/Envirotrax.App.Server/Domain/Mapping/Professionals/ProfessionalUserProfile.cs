
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Data.Models.Users;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalUserProfile : Profile
{
    public ProfessionalUserProfile()
    {
        CreateMap<ProfessionalUser, ProfessionalUserDto>()
            .ForMember(proUser => proUser.Id, opt => opt.MapFrom(proUser => proUser.UserId))
            .ForMember(proUser => proUser.EmailAddress, opt => opt.MapFrom(proUser => proUser.User!.Email))
            .AfterMap((model, dto) =>
            {
                if (model.BillingStateId.HasValue)
                {
                    dto.BillingState ??= new()
                    {
                        Id = model.BillingStateId.Value
                    };
                }
            })
            .ReverseMap()
            .ForMember(proUser => proUser.User, opt => opt.Ignore())
            .ForMember(proUser => proUser.UserId, opt => opt.MapFrom(proUser => proUser.Id))
            .ForMember(proUser => proUser.BillingState, opt => opt.Ignore())
            .ForMember(proUser => proUser.BillingStateId, opt => opt.MapFrom(proUser => proUser.BillingState!.Id));

        CreateMap<AppUser, ReferencedProfessionalUserDto>()
               .ForMember(d => d.EmailAddress, opt => opt.MapFrom(s => s.Email))
               .ForMember(d => d.ContactName, opt => opt.Ignore());

        CreateMap<ProfessionalUser, ReferencedProfessionalUserDto>()
             .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId))
             .ForMember(d => d.EmailAddress, opt => opt.MapFrom(s => s.User != null ? s.User.Email : null));
    }
}
