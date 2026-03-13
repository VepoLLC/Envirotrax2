
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Data.Services.Implementations;
using OpenIddict.Abstractions;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    private readonly IWaterSupplierUserRepository _supplierUserRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IFeatureRepository _featureRepository;

    public AuthService(
        IHttpContextAccessor contextAccessor,
        IWaterSupplierUserRepository supplierUserRepository,
        IProfessionalUserRepository professionalUserRepository,
        IFeatureRepository featureRepository)
        : base(contextAccessor)
    {
        _supplierUserRepository = supplierUserRepository;
        _professionalUserRepository = professionalUserRepository;
        _featureRepository = featureRepository;
    }

    public async Task SetSecurityPropertiesAsync(ClaimsPrincipal principal, int userId, int? waterSupplierId, int? professionalId)
    {
        var userAccess = await GetAccessSettingsAsync(userId, waterSupplierId, professionalId);

        if (userAccess.WaterSupplierId.HasValue && userAccess.WaterSupplierIdRequested.HasValue)
        {
            SetWaterSupplier(principal, userAccess.WaterSupplierIdRequested.Value);

            var permissions = await _supplierUserRepository.GetAllPermissionsAsync(userAccess.WaterSupplierId.Value, userId);
            SetPermissions(principal, permissions);
        }

        if (userAccess.ProfessionalId.HasValue && userAccess.ProfessionalIdRequested.HasValue)
        {
            SetProfessional(principal, userAccess.ProfessionalIdRequested.Value);
        }

        var features = await _featureRepository.GetAllAsync(userAccess.WaterSupplierIdRequested, userAccess.ProfessionalIdRequested);

        AddClaim(principal, "fts", string.Join(',', features.Select(f => (int)f)));
    }

    private void AddClaim(ClaimsPrincipal principal, string claimType, string value)
    {
        if (principal.Identity is ClaimsIdentity claimsIdentity)
        {
            claimsIdentity.AddClaim(new Claim(claimType, value).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        }
    }

    private async Task<UserAccessDto> GetAccessSettingsAsync(int userId, int? waterSupplierId, int? professionalid)
    {
        var accessDto = new UserAccessDto();

        if (waterSupplierId.HasValue)
        {
            var supplierUser = await _supplierUserRepository.GetAsync(waterSupplierId.Value, userId) ?? throw new ValidationException("User doesn't have access to this water supplier.");

            accessDto.WaterSupplierId = supplierUser.WaterSupplierId;
            accessDto.WaterSupplierIdRequested = waterSupplierId;
        }

        if (professionalid.HasValue)
        {
            var professionalUser = await _professionalUserRepository.GetAsync(professionalid.Value, userId) ?? throw new ValidationException("User doesn't have access to this registered professional.");

            accessDto.ProfessionalId = professionalUser.ProfessionalId;
            accessDto.ProfessionalIdRequested = professionalid;
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
                CanCreate = group.Any(p => p.CanCreate),
                CanEdit = group.Any(p => p.CanEdit),
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

            if (rolePermission.CanCreate)
            {
                action = action | PermissionAction.CanCreate;
            }

            if (rolePermission.CanEdit)
            {
                action = action | PermissionAction.CanEdit;
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

    public void SetPermissions(ClaimsPrincipal principal, IEnumerable<RolePermission> permissions)
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