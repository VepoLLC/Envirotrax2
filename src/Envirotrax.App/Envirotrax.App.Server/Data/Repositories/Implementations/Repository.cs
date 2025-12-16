

using System.Reflection;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

public abstract class Repository<TModel> : Repository<TModel, TenantDbContext>, IRepository<TModel>
        where TModel : class
{
    public Repository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector.Current)
    {
    }
}

public abstract class Repository<TModel, TDbContext> : Repository<TModel, int, TDbContext>, IRepository<TModel>
        where TModel : class
        where TDbContext : DbContext
{
    public Repository(TDbContext dbContext)
        : base(dbContext)
    {
    }
}


public abstract class Repository<TModel, TKey, TDbContext> : IRepository<TModel, TKey>
    where TModel : class
    where TDbContext : DbContext
{
    private readonly string _primaryKeyName;

    protected TDbContext Context { get; private set; }

    protected DbSet<TModel> Entity { get; private set; }

    public Repository(TDbContext dbContext)
    {
        Context = dbContext;
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
        return Context
            .Model
            .FindEntityType(typeof(TModel))!
            .FindPrimaryKey()!
            .Properties
            .First(p => p.PropertyInfo!.GetCustomAttribute<AppPrimaryKeyAttribute>()?.IsShadowKey == false)
            .Name;
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

    public virtual async Task<IEnumerable<TModel>> GetAllAsync()
    {
        return await GetListQuery().ToListAsync();
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query)
    {
        var paginated = await GetListQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo);

        return await paginated.ToListAsync();
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize)
    {
        var paginated = await GetListQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, maxPageSize);

        return await paginated.ToListAsync();
    }

    public virtual async Task<TModel?> GetNoIncludesAsync(TKey id)
    {
        return await Entity.SingleOrDefaultAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id));
    }

    public virtual async Task<TModel?> GetAsync(TKey id)
    {
        return await GetDetailsQuery().SingleOrDefaultAsync(m => EF.Property<TKey>(m, _primaryKeyName)!.Equals(id));
    }

    public virtual async Task<TModel> AddAsync(TModel model)
    {
        Entity.Add(model);
        await Context.SaveChangesAsync();

        return model;
    }

    protected virtual void UpdateEntity(TModel model)
    {
        Context.Attach(model);
        Entity.Entry(model).State = EntityState.Modified;
    }

    public virtual async Task<TModel?> UpdateAsync(TModel model)
    {
        UpdateEntity(model);

        await Context.SaveChangesAsync();

        return model;
    }

    public virtual async Task<TModel?> DeleteAsync(TKey id)
    {
        var model = await GetAsync(id);

        if (model != null)
        {
            Context.Entry(model).State = EntityState.Deleted;

            if (await Context.SaveChangesAsync() > 0)
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
            Context.Entry(model).State = EntityState.Detached;
            Context.Attach(model);

            var deletedTime = Context.Entry(model).Property(nameof(IAuditableModel<AspNetUserBase>.DeletedTime));
            deletedTime.CurrentValue = null;
            deletedTime.IsModified = true;

            var deletedById = Context.Entry(model).Property(nameof(IAuditableModel<AspNetUserBase>.DeletedById));
            deletedById.CurrentValue = null;
            deletedById.IsModified = true;

            await Context.SaveChangesAsync();

            return model;
        }

        return null;
    }

    public virtual async Task<TModel?> ReactivateAsync(TKey id)
    {
        if (ImplementsInterface(typeof(TModel), typeof(IDeleteAutitableModel<>)))
        {
            throw new InvalidOperationException($"An entity must implement {nameof(IDeleteAutitableModel<AspNetUserBase>)} interface for reactivating.");
        }

        var model = await GetAsync(id);

        return await ReactivateAsync(model);
    }
}