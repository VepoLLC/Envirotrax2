
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IService<TModel, TDto> : IService<TDto>
        where TModel : class
        where TDto : class
{
}

public interface IService<TDto> : IServiceBase<TDto, int>
    where TDto : class
{
}

public interface IService<TModel, TDto, TKey> : IServiceBase<TDto, TKey>
    where TModel : class
    where TDto : class
{
}

public interface IServiceBase<TDto, TKey>
    where TDto : class
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<int> CountAsync(Query query, CancellationToken cancellationToken);

    Task<TDto?> GetAsync(TKey id, CancellationToken cancellationToken);

    /// <summary>
    /// Returns whether a record with this id exists, without reading it. Use this instead of calling
    /// <see cref="GetAsync"/> and null-checking the result when the record itself isn't needed.
    /// </summary>
    Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken);

    Task<TDto> AddAsync(TDto dto);
    Task<TDto> UpdateAsync(TDto dto);
    Task<TDto?> DeleteAsync(TKey id);
    Task<TDto?> ReactivateAsync(TKey id);
}