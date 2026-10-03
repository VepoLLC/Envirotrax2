using Envirotrax.App.Server.Data.Models.Api;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Api;

public class ApiSupplierScopeService : IApiSupplierScopeService
{
    private readonly IWaterSupplierRepository _waterSupplierRepository;

    public ApiSupplierScopeService(IWaterSupplierRepository waterSupplierRepository)
    {
        _waterSupplierRepository = waterSupplierRepository;
    }

    public async Task<ApiSupplierScope?> ResolveAsync(ApiAccount account, CancellationToken cancellationToken)
    {
        // V1 treats WaterSupplierID = 0 as an account that is not bound to a supplier at all.
        if (account.WaterSupplierId == 0)
        {
            return ApiSupplierScope.Open();
        }

        var supplier = await _waterSupplierRepository.GetUnscopedAsync(account.WaterSupplierId, cancellationToken);

        if (supplier == null)
        {
            return null;
        }

        // Legacy MasterWaterSupplierID maps to WaterSupplier.ParentId. A supplier that has a parent
        // is itself a child, so the account is scoped to that supplier alone.
        if (supplier.ParentId != null && supplier.ParentId != 0)
        {
            return ApiSupplierScope.Single(account.WaterSupplierId);
        }

        var childWaterSupplierIds = await _waterSupplierRepository.GetChildSupplierIdsAsync(account.WaterSupplierId, cancellationToken);
        var childIds = childWaterSupplierIds.ToList();

        // V1 promotes a top-level supplier to master only when it actually has children.
        if (childIds.Count > 0)
        {
            return ApiSupplierScope.Master(account.WaterSupplierId, childIds);
        }

        return ApiSupplierScope.Single(account.WaterSupplierId);
    }
}
