using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Users;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Users;

public interface IUserRepository : IRepository<WaterSupplierUser>
{
    Task<IEnumerable<WaterSupplierUser>> GetAllForWaterSupplierAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
