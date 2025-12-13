
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
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query);
    Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize);

    Task<TDto?> GetAsync(TKey id);
    Task<TDto> AddAsync(TDto dto);
    Task<TDto> UpdateAsync(TDto dto);
    Task<TDto?> DeleteAsync(TKey id);
    Task<TDto?> ReactivateAsync(TKey id);
}