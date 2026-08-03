using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
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
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role);
    }

    public async Task<IEnumerable<WaterSupplierUser>> GetAllForWaterSupplierAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await GetListQuery()
            .Where(user => user.WaterSupplierId == waterSupplierId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
