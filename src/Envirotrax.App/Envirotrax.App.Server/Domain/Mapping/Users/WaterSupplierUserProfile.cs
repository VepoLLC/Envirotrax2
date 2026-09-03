
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
            .ForMember(supplierUser => supplierUser.CellNumber, opt => opt.MapFrom(supplierUser => supplierUser.User!.PhoneNumber))
            .ForMember(supplierUser => supplierUser.Roles, opt => opt.MapFrom(supplierUser => (supplierUser.UserRoles ?? Enumerable.Empty<UserRole>())
                .Where(userRole => userRole.Role!.DeletedTime == null)
                .OrderBy(userRole => userRole.Role!.Name)
                .Select(userRole => userRole.Role!)))
            .ReverseMap()
            .ForMember(supplierUser => supplierUser.User, opt => opt.Ignore())
            .ForMember(supplierUser => supplierUser.UserRoles, opt => opt.Ignore())
            .ForMember(supplierUser => supplierUser.UserId, opt => opt.MapFrom(supplierUser => supplierUser.Id));

        CreateMap<WaterSupplierUser, ReferencedWaterSupplierUserDto>()
            .ForMember(supplierUser => supplierUser.Id, opt => opt.MapFrom(supplierUser => supplierUser.UserId));
    }
}