
using System.ComponentModel.DataAnnotations;
using Envirotrax.Auth.Areas.OpenIdConnect.Services.Definitions;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Common.Data.Services.Implementations;
using Microsoft.CodeAnalysis.Elfie.Serialization;

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

    public async Task<UserAccessDto> GetAccessSettingsAsync(int userId, int? waterSupplierId, int? contractorId)
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