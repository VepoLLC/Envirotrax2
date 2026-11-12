
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Data.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    private readonly IWaterSupplierUserRepository _supplierUserRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly UserManager<AppUser> _userManager;

    public AuthService(
        IHttpContextAccessor contextAccessor,
        IWaterSupplierUserRepository supplierUserRepository,
        IProfessionalUserRepository professionalUserRepository,
        IFeatureRepository featureRepository,
        UserManager<AppUser> userManager)
        : base(contextAccessor)
    {
        _supplierUserRepository = supplierUserRepository;
        _professionalUserRepository = professionalUserRepository;
        _featureRepository = featureRepository;
        _userManager = userManager;
    }

    public async Task SetSecurityPropertiesAsync(ClaimsPrincipal principal, int userId, int? waterSupplierId, int? professionalId, int? impersonatedUserId)
    {
        var userAccess = await GetAccessSettingsAsync(userId, waterSupplierId, professionalId, impersonatedUserId);

        if (userAccess.WaterSupplierId.HasValue && userAccess.WaterSupplierIdRequested.HasValue)
        {
            await SetWaterSupplierClaimsAsync(principal, userId, userAccess);
        }

        if (userAccess.ProfessionalId.HasValue && userAccess.ProfessionalIdRequested.HasValue)
        {
            SetProfessionalClaims(principal, userAccess);
        }

        if (userAccess.ImpersonatedUserId.HasValue)
        {
            SetImpersonatedUser(principal, userAccess.ImpersonatedUserId.Value, userAccess.ImpersonatedUserName!);
        }

        var features = await _featureRepository.GetAllAsync(userAccess.WaterSupplierId, userAccess.WaterSupplierIdRequested, userAccess.ProfessionalIdRequested);

        AddClaim(principal, "fts", string.Join(',', features.Select(f => (int)f)));
    }

    public async Task ValidateImpersonationAsync(ClaimsPrincipal principal, int userId)
    {
        var impersonatedUserIdClaim = principal.FindFirstValue(ImpersonatedUserIdClaim);

        if (string.IsNullOrWhiteSpace(impersonatedUserIdClaim))
        {
            return;
        }

        var impersonatedUserId = int.Parse(impersonatedUserIdClaim);
        var professionalId = int.Parse(principal.FindFirstValue("prfId")!);

        await GetAccessSettingsAsync(userId, null, professionalId, impersonatedUserId);
    }

    private async Task<AppUser> GetImpersonatedUserAsync(int userId, int impersonatedUserId, int? waterSupplierId, int? professionalId)
    {
        if (waterSupplierId.HasValue || !professionalId.HasValue)
        {
            throw new ValidationException("Users can be impersonated only within a registered professional.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || !user.IsSuperUser)
        {
            throw new ValidationException("User doesn't have access to impersonate other users.");
        }

        var impersonatedUser = await _userManager.FindByIdAsync(impersonatedUserId.ToString()) ?? throw new ValidationException("The user to impersonate doesn't exist.");

        if (impersonatedUser.IsSuperUser)
        {
            throw new ValidationException("Super users can't be impersonated.");
        }

        return impersonatedUser;
    }

    private async Task SetWaterSupplierClaimsAsync(ClaimsPrincipal principal, int userId, UserAccessDto userAccess)
    {
        SetWaterSupplier(principal, userAccess.WaterSupplierIdRequested!.Value);
        SetParentWaterSupplier(principal, userAccess.WaterSupplierId!.Value);

        var permissions = await _supplierUserRepository.GetAllPermissionsAsync(userAccess.WaterSupplierId!.Value, userId);

        SetPermissions(principal, permissions);
        AddClaim(principal, OpenIddictConstants.Claims.Role, RoleDefinitions.WaterSupplier);
    }

    private void SetProfessionalClaims(ClaimsPrincipal principal, UserAccessDto userAccess)
    {
        SetProfessional(principal, userAccess.ProfessionalIdRequested!.Value);
        AddClaim(principal, OpenIddictConstants.Claims.Role, RoleDefinitions.Professional);

        foreach (var role in userAccess.ProfessionalRoles)
        {
            AddClaim(principal, OpenIddictConstants.Claims.Role, role);
        }
    }

    private void AddClaim(ClaimsPrincipal principal, string claimType, string value)
    {
        if (principal.Identity is ClaimsIdentity claimsIdentity)
        {
            claimsIdentity.AddClaim(new Claim(claimType, value).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        }
    }

    private async Task<UserAccessDto> GetAccessSettingsAsync(int userId, int? waterSupplierId, int? professionalid, int? impersonatedUserId)
    {
        var accessDto = new UserAccessDto();
        var professionalUserId = userId;

        if (impersonatedUserId.HasValue)
        {
            var impersonatedUser = await GetImpersonatedUserAsync(userId, impersonatedUserId.Value, waterSupplierId, professionalid);

            accessDto.ImpersonatedUserId = impersonatedUser.Id;
            accessDto.ImpersonatedUserName = impersonatedUser.UserName;

            professionalUserId = impersonatedUser.Id;
        }

        if (waterSupplierId.HasValue)
        {
            var supplierUser = await _supplierUserRepository.GetAsync(waterSupplierId.Value, userId) ?? throw new ValidationException("User doesn't have access to this water supplier.");

            accessDto.WaterSupplierId = supplierUser.WaterSupplierId;
            accessDto.WaterSupplierIdRequested = waterSupplierId;
        }

        if (professionalid.HasValue)
        {
            var professionalUser = await _professionalUserRepository.GetAsync(professionalid.Value, professionalUserId) ?? throw new ValidationException("User doesn't have access to this registered professional.");

            accessDto.ProfessionalId = professionalUser.ProfessionalId;
            accessDto.ProfessionalIdRequested = professionalid;

            if (professionalUser.IsAdmin)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.Admin);
            }

            if (professionalUser.IsWiseGuy)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.WiseGuy);
            }

            if (professionalUser.IsCsiInspector)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.CsiInspector);
            }

            if (professionalUser.IsBackflowTester)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.BackflowTester);
            }

            if (professionalUser.IsFogInspector)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.FogInspector);
            }

            if (professionalUser.IsFogTransporter)
            {
                accessDto.ProfessionalRoles.Add(RoleDefinitions.Professionals.FogTransporter);
            }
        }

        return accessDto;
    }

    private string GetPermissionString(IEnumerable<RolePermission> rolePermissions)
    {
        var sb = new StringBuilder();

        var list = rolePermissions
            .GroupBy(p => p.PermissionId)
            .Select(group => new RolePermission
            {
                PermissionId = group.Key,
                CanView = group.Any(p => p.CanView),
                CanModify = group.Any(p => p.CanModify),
                CanDelete = group.Any(p => p.CanDelete)
            }).ToList();


        for (int i = 0; i < list.Count; i++)
        {
            var rolePermission = list[i];
            var action = PermissionAction.None;

            sb.AppendFormat("{0}=", rolePermission.PermissionId!);

            if (rolePermission.CanView)
            {
                action = action | PermissionAction.CanView;
            }

            if (rolePermission.CanModify)
            {
                action = action | PermissionAction.CanModify;
            }

            if (rolePermission.CanDelete)
            {
                action = action | PermissionAction.CanDelete;
            }

            sb.Append((int)action);

            // If it is not the last item
            if (i < list.Count - 1)
            {
                sb.Append(',');
            }
        }

        return sb.ToString();
    }

    private void SetPermissions(ClaimsPrincipal principal, IEnumerable<RolePermission> permissions)
    {
        if (permissions != null && permissions.Any())
        {
            if (principal.Identity is ClaimsIdentity identity)
            {
                var permissionString = GetPermissionString(permissions);
                identity.AddClaim(new Claim("prms", permissionString).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }
        }
    }
}