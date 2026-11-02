
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IWaterSupplierRepository : IRepository<WaterSupplier>
{
    Task<IEnumerable<WaterSupplier>> GetAllMySuppliersAsync(CancellationToken cancellationToken);

    Task<IEnumerable<int>> GetSupplierIdsAsync(bool hasBackflowTests, CancellationToken cancellationToken);

    Task<IEnumerable<WaterSupplierLetterSettingsDto>> GetAllLetterSettingsAsync(CancellationToken cancellationToken);

    Task<WaterSupplier?> GetUnscopedAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<IEnumerable<int>> GetChildSupplierIdsAsync(int parentWaterSupplierId, CancellationToken cancellationToken);

    Task<WaterSupplier?> UpdateOwnAsync(WaterSupplier supplier);
}