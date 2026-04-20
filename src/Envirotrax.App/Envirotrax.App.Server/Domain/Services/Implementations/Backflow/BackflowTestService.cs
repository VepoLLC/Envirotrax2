using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : Service<BackflowTest, BackflowTestDto>, IBackflowTestService
{
    private readonly IBackflowTestRepository _backflowTestRepository;

    public BackflowTestService(IMapper mapper, IBackflowTestRepository repository)
        : base(mapper, repository)
    {
        _backflowTestRepository = repository;
    }

    public override async Task<IPagedData<BackflowTestDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }

        var facilityTypeProps = query.Filter
            .Where(f => f.ColumnName?.Equals("facilityType", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        query.Sort = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);

        var facilityTypes = facilityTypeProps
            .SelectMany(f => f.Children ?? Enumerable.Empty<QueryProperty>())
            .Where(c => int.TryParse(c.Value?.ToString(), out _))
            .Select(c => (FacilityType)int.Parse(c.Value!.ToString()!))
            .Distinct()
            .ToList();

        var model = await _backflowTestRepository.GetAllWithFacilityTypesAsync(pageInfo, query, facilityTypes, cancellationToken);

        return model.Select(m => MapToDto(m)!).ToPagedData(pageInfo);
    }
}
