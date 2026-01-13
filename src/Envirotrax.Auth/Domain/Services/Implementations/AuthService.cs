
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
    private readonly IContractorUserRepository _contractorUserRepository;

    public AuthService(
        IHttpContextAccessor contextAccessor,
        IWaterSupplierUserRepository supplierUserRepository,
        IContractorUserRepository contractorUserRepository)
        : base(contextAccessor)
    {
        _supplierUserRepository = supplierUserRepository;
        _contractorUserRepository = contractorUserRepository;
    }

    public async Task SetSecurityPropertiesAsync(ClaimsPrincipal principal, int userId, int? waterSupplierId, int? contractorId)
    {
        var userAccess = await GetAccessSettingsAsync(userId, waterSupplierId, contractorId);

        if (userAccess.WaterSupplierId.HasValue)
        {
            SetWaterSupplier(principal, userAccess.WaterSupplierId.Value);
        }

        if (userAccess.ContractorId.HasValue)
        {
            SetContractor(principal, userAccess.ContractorId.Value);
        }
    }

    private async Task<UserAccessDto> GetAccessSettingsAsync(int userId, int? waterSupplierId, int? contractorId)
    {
        var accessDto = new UserAccessDto();

        if (waterSupplierId.HasValue)
        {
            var supplierUser = await _supplierUserRepository.GetAsync(waterSupplierId.Value, userId) ?? throw new ValidationException("User doesn't have access to this water supplier.");
            accessDto.WaterSupplierId = supplierUser.WaterSupplierId;
        }

        if (contractorId.HasValue)
        {
            var contractorUser = await _contractorUserRepository.GetAsync(contractorId.Value, userId) ?? throw new ValidationException("User doesn't have access to this contractor.");
            accessDto.ContractorId = contractorUser.ContractorId;
        }

        return accessDto;
    }
}