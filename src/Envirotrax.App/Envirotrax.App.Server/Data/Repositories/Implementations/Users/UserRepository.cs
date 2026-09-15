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
            .Include(user => user.UserRoles!)
            .ThenInclude(userRole => userRole.Role)
            .Include(user => user.User);
    }

    protected override IQueryable<WaterSupplierUser> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(user => user.User);
    }

    public override Task<IEnumerable<WaterSupplierUser>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(WaterSupplierUser.UserId)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
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

    public async Task<UpdateResult<WaterSupplierUser>> UpdateUserAsync(WaterSupplierUser model)
    {
        var result = new UpdateResult<WaterSupplierUser>();

        var user = await GetTrackedForUpdateAsync(model.UserId, default);

        if (user == null)
        {
            return result;
        }

        user.ContactName = model.ContactName;
        user.EmailAddress = model.EmailAddress;

        result.Changes = BuildChangeDescription(user);

        await DbContext.SaveChangesAsync();

        result.Model = user;

        return result;
    }
}
