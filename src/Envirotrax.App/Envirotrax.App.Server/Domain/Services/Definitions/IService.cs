
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IService<TModel, TDto> : IService<TDto>
        where TModel : class
        where TDto : class, IDto
{
}

public interface IService<TDto> : IServiceBase<TDto, int>
    where TDto : class, IDto
{
}

public interface IService<TModel, TDto, TKey> : IServiceBase<TDto, TKey>
    where TModel : class
    where TDto : class, IDto<TKey>
{
}

public interface IServiceBase<TDto, TKey>
    where TDto : class, IDto<TKey>
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize, CancellationToken cancellationToken);

    Task<TDto?> GetAsync(TKey id, CancellationToken cancellationToken);
    Task<TDto> AddAsync(TDto dto);
    Task<TDto> UpdateAsync(TDto dto);
    Task<TDto?> DeleteAsync(TKey id);
    Task<TDto?> ReactivateAsync(TKey id);
}