
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
    Task<TModel?> GetAsync(TKey id, CancellationToken cancellationToken);
    Task<TModel?> GetNoIncludesAsync(TKey id, CancellationToken cancellationToken);
    Task<TModel> AddAsync(TModel model);
    Task<TModel?> UpdateAsync(TModel model);
    Task<TModel?> DeleteAsync(TKey id);
    Task<TModel?> ReactivateAsync(TKey id);
}