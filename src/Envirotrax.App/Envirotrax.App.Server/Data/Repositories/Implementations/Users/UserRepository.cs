using System.Transactions;
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
            .Include(user => user.User)
            .Where(user => user.DeletedTime == null);
    }

    protected override IQueryable<WaterSupplierUser> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(user => user.User)
            .Where(user => user.DeletedTime == null);
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

    public async Task<WaterSupplierUser?> UpdateUserAsync(WaterSupplierUser model)
    {
        var user = await Entity.SingleOrDefaultAsync(u => u.UserId == model.UserId && u.DeletedTime == null);

        if (user == null)
        {
            return null;
        }

        user.ContactName = model.ContactName;
        user.EmailAddress = model.EmailAddress;

        await SaveChangesAsync(logData: true);

        return user;
    }

    public async Task<WaterSupplierUser?> RestoreDeletedAsync(WaterSupplierUser user, CancellationToken cancellationToken)
    {
        var deletedUser = Entity.Where(u => u.UserId == user.UserId && u.DeletedTime != null);

        if (!await deletedUser.AnyAsync(cancellationToken))
        {
            return null;
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        // SaveChanges cannot clear these: AuditEntities keeps them unmodified on every save except a delete.
        await deletedUser.ExecuteUpdateAsync(set => set
            .SetProperty(u => u.DeletedTime, (DateTime?)null)
            .SetProperty(u => u.DeletedById, (int?)null), cancellationToken);

        var restored = await Entity.SingleAsync(u => u.UserId == user.UserId, cancellationToken);

        restored.ContactName = user.ContactName;
        restored.EmailAddress = user.EmailAddress;

        await SaveChangesAsync(logData: true, cancellationToken);

        scope.Complete();

        return restored;
    }
}
