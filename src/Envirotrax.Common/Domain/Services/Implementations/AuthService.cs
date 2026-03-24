
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Implementations;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthService(IHttpContextAccessor contextAccessor)
        : base(contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public IEnumerable<FeatureType> GetAllMyFeatures()
    {
        var claim = _contextAccessor.HttpContext?.User.FindFirstValue("fts");

        if (!string.IsNullOrWhiteSpace(claim))
        {
            return claim
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => (FeatureType)int.Parse(f));
        }

        return [];
    }

    public bool HasAnyFeatures(params FeatureType[] featuresToCheck)
    {
        var features = GetAllMyFeatures();
        return features.Any(f => featuresToCheck.Contains(f));
    }

    private RolePermissionDto ParseOnePermission(string permissionString)
    {
        var permission = new RolePermissionDto();
        var parts = permissionString.Split('=');

        permission.Permission = Enum.Parse<PermissionType>(parts[0]);

        var permissionAction = (PermissionAction)Enum.ToObject(typeof(PermissionAction), int.Parse(parts[1]));

        if (permissionAction.HasFlag(PermissionAction.CanView))
        {
            permission.CanView = true;
        }

        if (permissionAction.HasFlag(PermissionAction.CanCreate))
        {
            permission.CanCreate = true;
        }

        if (permissionAction.HasFlag(PermissionAction.CanEdit))
        {
            permission.CanEdit = true;
        }

        if (permissionAction.HasFlag(PermissionAction.CanDelete))
        {
            permission.CanDelete = true;
        }

        return permission;
    }

    private IEnumerable<RolePermissionDto> ParsePermissions(string? permissionsString)
    {
        if (!string.IsNullOrWhiteSpace(permissionsString))
        {
            return permissionsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => ParseOnePermission(p));
        }

        return [];
    }

    public IEnumerable<RolePermissionDto> GetAllMyPermissions()
    {
        if (_contextAccessor.HttpContext!.Items.TryGetValue("prms", out var permissions))
        {
            return (IEnumerable<RolePermissionDto>)permissions!;
        }

        var parsedPermissions = ParsePermissions(_contextAccessor.HttpContext?.User.FindFirstValue("prms"));
        _contextAccessor.HttpContext!.Items["prms"] = parsedPermissions;

        return parsedPermissions;
    }

    private bool HasPermission(IEnumerable<RolePermissionDto> permissions, PermissionType type, PermissionAction action)
    {
        var permissionToCheck = permissions.SingleOrDefault(p => p.Permission == type);

        if (permissionToCheck != null)
        {
            foreach (var value in Enum.GetValues<PermissionAction>())
            {
                if (action.HasFlag(value))
                {
                    switch (value)
                    {
                        case PermissionAction.CanView:
                            return permissionToCheck.CanView;
                        case PermissionAction.CanCreate:
                            return permissionToCheck.CanCreate;
                        case PermissionAction.CanEdit:
                            return permissionToCheck.CanEdit;
                        case PermissionAction.CanDelete:
                            return permissionToCheck.CanDelete;
                    }
                }
            }
        }

        return false;
    }

    public bool HasAnyPermission(PermissionAction action, params PermissionType[] permissionTypes)
    {
        var permissions = GetAllMyPermissions();

        foreach (var type in permissionTypes)
        {
            if (HasPermission(permissions, type, action))
            {
                return true;
            }
        }

        return false;
    }

    public IEnumerable<string> GetAllMyRoles()
    {
        return _contextAccessor.HttpContext?.User
            .FindAll(OpenIddictConstants.Claims.Role)
            .Select(c => c.Value)
            ?? [];
    }
}