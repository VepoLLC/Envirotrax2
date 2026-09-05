
using DeveloperPartners.SortingFiltering;

namespace Envirotrax.App.Server.Data.Repositories.Definitions;

public interface IRepository<TModel> : IRepository<TModel, int>
    where TModel : class
{

}

public interface IRepository<TModel, TKey>
    where TModel : class
{
    Task<IEnumerable<TModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<TModel>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(Query query, CancellationToken cancellationToken);

    Task<TModel?> GetAsync(TKey id, CancellationToken cancellationToken);
    Task<TModel?> GetNoIncludesAsync(TKey id, CancellationToken cancellationToken);

    /// <summary>
    /// Loads the entity TRACKED (unlike <see cref="GetAsync"/>/<see cref="GetNoIncludesAsync"/>, which use
    /// AsNoTracking) so the caller can mutate a subset of fields and persist via <see cref="SaveChangesAsync"/>
    /// without a full-entity overwrite.
    /// </summary>
    Task<TModel?> GetTrackedForUpdateAsync(TKey id, CancellationToken cancellationToken);

    // Deliberately takes no CancellationToken: once a save is in flight, cancelling the request
    // shouldn't be able to abort a write partway through and leave the database inconsistent.
    Task SaveChangesAsync();

    Task<TModel> AddAsync(TModel model);
    Task<TModel?> UpdateAsync(TModel model);
    Task<TModel?> DeleteAsync(TKey id);
    Task<TModel?> ReactivateAsync(TKey id);
}