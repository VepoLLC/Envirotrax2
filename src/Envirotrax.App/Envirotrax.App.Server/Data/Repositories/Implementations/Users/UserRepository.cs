
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Users;

public class UserRepository : Repository<WaterSupplierUser>, IUserRepository
{
    public UserRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<WaterSupplierUser> GetListQuery()
    {
        return base.GetListQuery()
            .Include(user => user.UserRoles!)
            .ThenInclude(userRole => userRole.Role);
    }

    public async Task<IEnumerable<WaterSupplierUser>> GetAllForWaterSupplierAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(user => user.WaterSupplierId == waterSupplierId)
            .OrderBy(user => user.ContactName)
            .ToListAsync(cancellationToken);
    }
}
