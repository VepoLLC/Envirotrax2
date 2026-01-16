
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Mapping.Users;

public class WaterSupplierUserProfile : Profile
{
    public WaterSupplierUserProfile()
    {
        CreateMap<WaterSupplierUser, WaterSupplierUserDto>()
            .ForMember(supplierUser => supplierUser.Id, opt => opt.MapFrom(supplierUser => supplierUser.UserId))
            .ReverseMap()
            .ForMember(supplierUser => supplierUser.User, opt => opt.Ignore())
            .ForMember(supplierUser => supplierUser.UserId, opt => opt.MapFrom(supplierUser => supplierUser.Id));
    }
}