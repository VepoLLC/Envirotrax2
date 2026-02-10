
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
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

        if (userAccess.WaterSupplierId.HasValue)
        {
            SetWaterSupplier(principal, userAccess.WaterSupplierId.Value);
        }

        if (userAccess.ProfessionalId.HasValue)
        {
            SetProfessional(principal, userAccess.ProfessionalId.Value);
        }

        var features = await _featureRepository.GetAllAsync(userAccess.WaterSupplierId, userAccess.ProfessionalId);

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
        }

        if (professionalid.HasValue)
        {
            var professionalUser = await _professionalUserRepository.GetAsync(professionalid.Value, userId) ?? throw new ValidationException("User doesn't have access to this registered professional.");
            accessDto.ProfessionalId = professionalUser.ProfessionalId;
        }

        return accessDto;
    }
}