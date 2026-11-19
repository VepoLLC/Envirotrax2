using AutoMapper;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;

namespace Envirotrax.App.Server.Domain.Mapping.Crm;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(dto => dto.State, opt => opt.Ignore())
            .ForMember(dto => dto.NextContactDateStatus, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                if (model.StateId.HasValue)
                {
                    dto.State ??= new() { Id = model.StateId.Value };
                }

                if (model.LastContactedById.HasValue)
                {
                    dto.LastContactedBy ??= new() { Id = model.LastContactedById.Value };
                }
            })
            .ReverseMap()
            .ForMember(model => model.State, opt => opt.Ignore())
            .ForMember(model => model.StateId, opt => opt.MapFrom(dto => dto.State != null ? dto.State.Id : null))
            .ForMember(model => model.LastContactedBy, opt => opt.Ignore())
            .ForMember(model => model.LastContactedById, opt => opt.MapFrom(dto => dto.LastContactedBy != null ? dto.LastContactedBy.Id : (int?)null))
            .ForMember(model => model.CreatedBy, opt => opt.Ignore())
            .ForMember(model => model.UpdatedBy, opt => opt.Ignore())
            .ForMember(model => model.DeletedBy, opt => opt.Ignore());
    }
}
