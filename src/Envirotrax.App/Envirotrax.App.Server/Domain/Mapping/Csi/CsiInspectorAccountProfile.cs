
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectorAccountProfile : Profile
{
    public CsiInspectorAccountProfile()
    {
        CreateMap<ProfessionalUser, CsiInspectorAccountDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(model => model.UserId))
            .ForMember(dto => dto.IsMasterAccount, opt => opt.MapFrom(model => model.IsAdmin))
            .ForMember(dto => dto.EmailAddress, opt => opt.MapFrom(model => model.User!.Email))
            .ForMember(dto => dto.CompanyName, opt => opt.MapFrom(model => model.Professional!.Name))
            .ForMember(dto => dto.Address, opt => opt.MapFrom(model => model.Professional!.Address))
            .ForMember(dto => dto.City, opt => opt.MapFrom(model => model.Professional!.City))
            .ForMember(dto => dto.State, opt => opt.MapFrom(model => model.Professional!.State))
            .ForMember(dto => dto.ZipCode, opt => opt.MapFrom(model => model.Professional!.ZipCode))
            .ForMember(dto => dto.WorkNumber, opt => opt.MapFrom(model => model.Professional!.PhoneNumber));
    }
}
