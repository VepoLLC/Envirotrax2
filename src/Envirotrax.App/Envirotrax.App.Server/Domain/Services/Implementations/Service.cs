

using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.Services.Definitions;

public class Service<TModel, TDto> : Service<TModel, TDto, int>
        where TModel : class
        where TDto : class
{
    public Service(IMapper mapper, IRepository<TModel> repository)
        : base(mapper, repository)
    {
    }
}

public class Service<TModel, TDto, TKey> : IService<TModel, TDto, TKey>
    where TModel : class
    where TDto : class
{
    protected IMapper Mapper { get; private set; }
    protected IRepository<TModel, TKey> Repository { get; private set; }

    public Service(IMapper mapper, IRepository<TModel, TKey> repository)
    {
        Mapper = mapper;
        Repository = repository;
    }

    protected virtual TDto? MapToDto(TModel? model)
    {
        if (model == null)
        {
            return null;
        }

        return Mapper.Map<TModel, TDto>(model);
    }

    protected virtual TModel? MapToModel(TDto? dto)
    {
        if (dto == null)
        {
            return null;
        }

        return Mapper.Map<TDto, TModel>(dto);
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var model = await Repository.GetAllAsync(cancellationToken);
        return model.Select(m => MapToDto(m)!);
    }

    public virtual async Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<TModel, TDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<TModel, TDto>(Mapper);

        var model = await Repository.GetAllAsync(pageInfo, query, cancellationToken);

        return model
            .Select(m => MapToDto(m)!)
            .ToPagedData(pageInfo);
    }

    public virtual async Task<IPagedData<TDto>> GetAllAsync(PageInfo pageInfo, Query query, int maxPageSize, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<TModel, TDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<TModel, TDto>(Mapper);

        var model = await Repository.GetAllAsync(pageInfo, query, maxPageSize, cancellationToken);

        return model
            .Select(m => MapToDto(m)!)
            .ToPagedData(pageInfo);
    }

    public virtual async Task<TDto?> GetNoIncludesAsync(TKey id, CancellationToken cancellationToken)
    {
        var model = await Repository.GetNoIncludesAsync(id, cancellationToken);
        return MapToDto(model);
    }

    public virtual async Task<TDto?> GetAsync(TKey id, CancellationToken cancellationToken)
    {
        var model = await Repository.GetAsync(id, cancellationToken);
        return MapToDto(model);
    }

    public virtual async Task<TDto> AddAsync(TDto dto)
    {
        var model = MapToModel(dto);
        var added = await Repository.AddAsync(model!);

        return MapToDto(model)!;
    }

    public virtual async Task<TDto> UpdateAsync(TDto dto)
    {
        var model = MapToModel(dto);
        var updated = await Repository.UpdateAsync(model!);

        return MapToDto(model)!;
    }

    public virtual async Task<TDto?> DeleteAsync(TKey id)
    {
        var deleted = await Repository.DeleteAsync(id);
        return MapToDto(deleted);
    }

    public virtual async Task<TDto?> ReactivateAsync(TKey id)
    {
        var reactivated = await Repository.ReactivateAsync(id);
        return MapToDto(reactivated);
    }
}