
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Common.Data.Services.Implementations;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class AuthService : TenantProviderService, IAuthService
{
    private readonly IWaterSupplierUserRepository _supplierUserRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;

    public AuthService(
        IHttpContextAccessor contextAccessor,
        IWaterSupplierUserRepository supplierUserRepository,
        IProfessionalUserRepository professionalUserRepository)
        : base(contextAccessor)
    {
        _supplierUserRepository = supplierUserRepository;
        _professionalUserRepository = professionalUserRepository;
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