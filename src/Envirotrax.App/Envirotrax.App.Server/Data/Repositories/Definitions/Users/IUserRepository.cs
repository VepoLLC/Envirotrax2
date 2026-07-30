
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Users;

public interface IUserRepository : IRepository<WaterSupplierUser>
{
    Task<IEnumerable<AdminWaterSupplierUserAccountDto>> GetAccountsWithPermissionsAsync(int waterSupplierId, CancellationToken cancellationToken);
}
