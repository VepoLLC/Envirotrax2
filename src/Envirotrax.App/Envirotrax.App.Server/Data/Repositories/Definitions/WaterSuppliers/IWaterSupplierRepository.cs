
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;

public interface IWaterSupplierRepository : IRepository<WaterSupplier>
{
    Task<IEnumerable<WaterSupplier>> GetAllMySuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}