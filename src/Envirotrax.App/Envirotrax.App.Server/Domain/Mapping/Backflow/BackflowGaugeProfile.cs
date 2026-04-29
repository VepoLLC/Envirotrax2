
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Mapping.Backflow;

public class BackflowGaugeProfile : Profile
{
    public BackflowGaugeProfile()
    {
        CreateMap<BackflowGauge, BackflowGaugeDto>()
            .ForMember(dto => dto.Professional, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Professional ??= new()
                {
                    Id = model.ProfessionalId
                };
            })
            .ReverseMap()
            .ForMember(g => g.Professional, opt => opt.Ignore())
            .ForMember(g => g.ProfessionalId, opt => opt.MapFrom(dto => dto.Professional != null ? dto.Professional.Id : (int?)null))
            .ForMember(g => g.CreatedBy, opt => opt.Ignore());

        CreateMap<CreateBackflowGaugeDto, BackflowGauge>();
    }
}
