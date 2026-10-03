using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data;
using Envirotrax.Common.Data.Services.Definitions;
using System.Transactions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class SettingsCopyService : ISettingsCopyService
{
    private readonly ITenantProvidersService _tenantProvider;
    private readonly IWaterSupplierRepository _waterSupplierRepository;
    private readonly IGeneralSettingsRepository _generalSettingsRepository;
    private readonly ICsiSettingsRepository _csiSettingsRepository;
    private readonly IBackflowSettingsRepository _backflowSettingsRepository;
    private readonly IBackflowRenewalRequirementRepository _renewalRequirementRepository;

    public SettingsCopyService(
        ITenantProvidersService tenantProvider,
        IWaterSupplierRepository waterSupplierRepository,
        IGeneralSettingsRepository generalSettingsRepository,
        ICsiSettingsRepository csiSettingsRepository,
        IBackflowSettingsRepository backflowSettingsRepository,
        IBackflowRenewalRequirementRepository renewalRequirementRepository)
    {
        _tenantProvider = tenantProvider;
        _waterSupplierRepository = waterSupplierRepository;
        _generalSettingsRepository = generalSettingsRepository;
        _csiSettingsRepository = csiSettingsRepository;
        _backflowSettingsRepository = backflowSettingsRepository;
        _renewalRequirementRepository = renewalRequirementRepository;
    }

    public async Task<bool> CanCopyFromParentAsync(CancellationToken cancellationToken)
    {
        var parentId = await GetCopySourceParentIdAsync(cancellationToken);

        return parentId.HasValue;
    }

    public async Task CopyFromParentAsync(SettingsSection section, CancellationToken cancellationToken)
    {
        var parentId = await GetCopySourceParentIdAsync(cancellationToken);

        if (!parentId.HasValue)
        {
            throw new AppValidationException("This water supplier has no parent account to copy settings from.");
        }

        var waterSupplierId = _tenantProvider.WaterSupplierId;

        switch (section)
        {
            case SettingsSection.General:
                await _generalSettingsRepository.CopyFromAsync(waterSupplierId, parentId.Value, section, cancellationToken);
                break;

            case SettingsSection.Csi:
            case SettingsSection.CsiLetterMessage:
                await _csiSettingsRepository.CopyFromAsync(waterSupplierId, parentId.Value, section, cancellationToken);
                break;

            case SettingsSection.Backflow:
                await CopyBackflowSettingsWithRenewalRequirementsAsync(waterSupplierId, parentId.Value, cancellationToken);
                break;

            case SettingsSection.BackflowLetterMessage:
                await _backflowSettingsRepository.CopyFromAsync(waterSupplierId, parentId.Value, section, cancellationToken);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }
    }

    private async Task CopyBackflowSettingsWithRenewalRequirementsAsync(int waterSupplierId, int parentId, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _backflowSettingsRepository.CopyFromAsync(waterSupplierId, parentId, SettingsSection.Backflow, cancellationToken);
        await _renewalRequirementRepository.ReplaceFromAsync(waterSupplierId, parentId, cancellationToken);

        scope.Complete();
    }

    private async Task<int?> GetCopySourceParentIdAsync(CancellationToken cancellationToken)
    {
        var supplier = await _waterSupplierRepository.GetUnscopedAsync(_tenantProvider.WaterSupplierId, cancellationToken);

        if (supplier?.ParentId == null)
        {
            return null;
        }

        var parent = await _waterSupplierRepository.GetUnscopedAsync(supplier.ParentId.Value, cancellationToken);

        if (parent == null || parent.Domain == WaterSupplier.EnvirotraxAdminDomain)
        {
            return null;
        }

        return parent.Id;
    }
}
