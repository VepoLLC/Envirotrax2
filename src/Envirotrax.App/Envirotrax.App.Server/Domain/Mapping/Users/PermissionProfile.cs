
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.Mapping.Users;

public class PermissionProfile : Profile
{
    public PermissionProfile()
    {
        CreateMap<Permission, ReferencedPermissionDto>()
            .ForMember(permission => permission.Category, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Category = ConvertCategory(model.Category);
            })
            .ReverseMap()
            .ForMember(permission => permission.Category, opt => opt.Ignore());

        CreateMap<Role, RoleDto>()
            .ReverseMap();

        CreateMap<Role, ReferencedRoleDto>()
            .ReverseMap();

        CreateMap<UserRole, UserRoleDto>()
            .AfterMap((model, dto) =>
            {
                dto.User ??= new()
                {
                    Id = model.UserId
                };

                dto.Role ??= new()
                {
                    Id = model.RoleId
                };
            })
            .ReverseMap()
            .ForMember(x => x.Role, opt => opt.Ignore())
            .ForMember(x => x.RoleId, opt => opt.MapFrom(x => x.Role.Id))
            .ForMember(x => x.User, opt => opt.Ignore())
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.User.Id));

        CreateMap<RolePermission, RolePermissionDto>()
            .AfterMap((model, dto) =>
            {
                dto.Permission ??= new()
                {
                    Id = model.PermissionId
                };

                dto.Role ??= new()
                {
                    Id = model.RoleId
                };
            })
            .ReverseMap()
            .ForMember(x => x.Role, opt => opt.Ignore())
            .ForMember(x => x.RoleId, opt => opt.MapFrom(x => x.Role.Id))
            .ForMember(x => x.Permission, opt => opt.Ignore())
            .ForMember(x => x.PermissionId, opt => opt.MapFrom(x => x.Permission.Id));
    }

    private string ConvertCategory(PermissionCategoryType categoryType)
    {
        switch (categoryType)
        {
            case PermissionCategoryType.General:
                return "General Permissions";
            case PermissionCategoryType.Backflow:
                return "Backflow Permissions";
            case PermissionCategoryType.Csi:
                return "CSI Permissions";
            case PermissionCategoryType.Fog:
                return "FOG Permissions";
            default:
                throw new InvalidOperationException("Unmapped permission category.");
        }
    }
}