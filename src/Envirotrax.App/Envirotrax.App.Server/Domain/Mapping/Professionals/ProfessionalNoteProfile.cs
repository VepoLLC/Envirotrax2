using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class ProfessionalNoteProfile : Profile
{
    public ProfessionalNoteProfile()
    {
        CreateMap<ProfessionalNote, ProfessionalNoteDto>()
            .ReverseMap()
            .ForMember(model => model.CreatedBy, opt => opt.Ignore());
    }
}
