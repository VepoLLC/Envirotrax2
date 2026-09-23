

using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Extensions;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

public abstract class Repository<TModel> : Repository<TModel, int, TenantDbContext>, IRepository<TModel>
        where TModel : class
{
    public Repository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector.Current)
    {
    }
}

public abstract class Repository<TModel, TKey> : Repository<TModel, TKey, TenantDbContext>, IRepository<TModel, TKey>
        where TModel : class
{
    public Repository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector.Current)
    {
    }
}

// TDbContext is constrained to TenantDbContext rather than DbContext so repositories can reach the
// record logging save (SaveChangesAndLogAsync). Every context in the app derives from it.
public abstract class Repository<TModel, TKey, TDbContext> : IRepository<TModel, TKey>
    where TModel : class
    where TDbContext : TenantDbContext
{
    private static readonly bool SupportsSoftDelete = ImplementsInterface(typeof(TModel), typeof(IDeleteAutitableModel<>));

    private readonly string _primaryKeyName;

    protected TDbContext DbContext { get; private set; }

    protected DbSet<TModel> Entity { get; private set; }

    public Repository(TDbContext dbContext)
    {
        DbContext = dbContext;
        Entity = dbContext.Set<TModel>();

        // Since our db primary key names can be different for each table, we get the primary key name from the model metadata.
        _primaryKeyName = GetPrimaryColumnName();
    }

    private static bool ImplementsInterface(Type type, Type openGenericInterface)
    {
        return type
            .GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface);
    }

    protected virtual string GetPrimaryColumnName()
    {
        return AppEntityKeys.GetPrimaryKeyName(DbContext.Model.FindEntityType(typeof(TModel))!);
    }

    /// <summary>
    /// Overrride this method to add joins (includes) for the <see cref="GetAllAsync"/> method.
    /// </summary>
    /// <returns></returns>
    protected virtual IQueryable<TModel> GetListQuery()
    {
        return Entity.AsNoTracking();
    }

    /// <summary>
    /// Override this method to add joins (includes) for the <see cref="GetAsync(int)"/> method.
    /// </summary>
    /// <returns></returns>
    protected virtual IQueryable<TModel> GetDetailsQuery()
    {
        return Entity.AsNoTracking();
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await GetListQuery().ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await GetListQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize, CancellationToken cancellationToken)
    {
        var paginated = await GetListQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, maxPageSize, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public virtual Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return CountAsync(new Query(), cancellationToken);
    }

    /// <summary>
    /// Counts the rows the list query would return, without fetching or materializing any of them.
    /// </summary>
    /// <remarks>
    /// This builds on <see cref="GetListQuery"/> so that every filter a repository applies there —
    /// tenant scoping, business rules — is honoured by the count too. Sorting and pagination are
    /// skipped because they cannot change a count, and EF Core drops the includes it doesn't need,
    /// so this is a single COUNT query rather than the two a paged read costs.
    ///
    /// Soft-deleted rows are left out the same way a list endpoint leaves them out: by filtering on
    /// DeletedTime. A query that already filters on DeletedTime — one model-bound from a request, or
    /// one that deliberately asks for deleted rows — is left as the caller built it.
    /// </remarks>
    public virtual Task<int> CountAsync(Query query, CancellationToken cancellationToken)
    {
        if (SupportsSoftDelete)
        {
            query.ExcludeDeleted();
        }

        return GetListQuery()
            .Where(query.Filter)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks for the row's existence with a single EXISTS query, without reading or materializing it.
    /// </summary>
    /// <remarks>
    /// This deliberately matches what <see cref="GetAsync"/> can find — the entity's query filters apply,
    /// but soft-deleted rows still count as existing — so it is a drop-in replacement for a GetAsync call
    /// that only exists to null-check.
    /// </remarks>
    public virtual Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken)
    {
        return Entity.AnyAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id), cancellationToken);
    }

    public virtual async Task<TModel?> GetNoIncludesAsync(TKey id, CancellationToken cancellationToken)
    {
        return await Entity
            .AsNoTracking()
            .SingleOrDefaultAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id), cancellationToken);
    }

    public virtual async Task<TModel?> GetAsync(TKey id, CancellationToken cancellationToken)
    {
        return await GetDetailsQuery()
            .AsNoTracking()
            .SingleOrDefaultAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id), cancellationToken);
    }

    public virtual async Task<TModel?> GetTrackedForUpdateAsync(TKey id, CancellationToken cancellationToken)
    {
        return await Entity.SingleOrDefaultAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id), cancellationToken);
    }

    public virtual Task SaveChangesAsync()
    {
        return DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Saves and, when <paramref name="logData"/> is true, writes the field-level record log for the
    /// changed entities in the same transaction — replacing a manual BuildChangeDescription call
    /// plus a RecordLogService.AddAsync call at the service layer.
    /// </summary>
    /// <remarks>
    /// Deliberately a separate method rather than an overload of the context's SaveChangesAsync:
    /// EF already owns SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken), and a
    /// second bool overload there would be resolved silently and mean something else entirely.
    ///
    /// Logging only covers entities marked with <see cref="RecordLoggedAttribute"/>, and only edits.
    /// Adds, deletes and any log that needs a written message still go through IRecordLogService.
    /// </remarks>
    protected virtual Task<int> SaveChangesAsync(bool logData, CancellationToken cancellationToken = default)
    {
        return logData
            ? DbContext.SaveChangesAndLogAsync(cancellationToken)
            : DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<TModel> AddAsync(TModel model)
    {
        Entity.Add(model);
        await DbContext.SaveChangesAsync();

        return model;
    }

    protected virtual void UpdateEntity(TModel model)
    {
        DbContext.Attach(model);
        Entity.Entry(model).State = EntityState.Modified;
    }

    public virtual async Task<TModel?> UpdateAsync(TModel model)
    {
        UpdateEntity(model);

        await DbContext.SaveChangesAsync();

        return model;
    }

    public virtual async Task<TModel?> DeleteAsync(TKey id)
    {
        var model = await GetAsync(id, default);

        if (model != null)
        {
            DbContext.Entry(model).State = EntityState.Deleted;

            if (await DbContext.SaveChangesAsync() > 0)
            {
                return model;
            }
        }

        return null;
    }

    protected virtual async Task<TModel?> ReactivateAsync(TModel? model)
    {
        if (model != null)
        {
            DbContext.Entry(model).State = EntityState.Detached;
            DbContext.Attach(model);

            var deletedTime = DbContext.Entry(model).Property(nameof(IAuditableModel<AspNetUserBase>.DeletedTime));
            deletedTime.CurrentValue = null;
            deletedTime.IsModified = true;

            var deletedById = DbContext.Entry(model).Property(nameof(IAuditableModel<AspNetUserBase>.DeletedById));
            deletedById.CurrentValue = null;
            deletedById.IsModified = true;

            await DbContext.SaveChangesAsync();

            return model;
        }

        return null;
    }

    public virtual async Task<TModel?> ReactivateAsync(TKey id)
    {
        if (!ImplementsInterface(typeof(TModel), typeof(IDeleteAutitableModel<>)))
        {
            throw new InvalidOperationException($"An entity must implement {nameof(IDeleteAutitableModel<AspNetUserBase>)} interface for reactivating.");
        }

        var model = await GetAsync(id, default);

        return await ReactivateAsync(model);
    }

}