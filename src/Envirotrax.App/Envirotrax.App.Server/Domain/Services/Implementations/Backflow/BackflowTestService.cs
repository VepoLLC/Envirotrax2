using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : Service<BackflowTest, BackflowTestDto>, IBackflowTestService
{
    private readonly IBackflowTestRepository _backflowTestRepository;
    private readonly IProfessionalService _professionalService;

    public BackflowTestService(IMapper mapper, IBackflowTestRepository repository, IProfessionalService professionalService)
        : base(mapper, repository)
    {
        _backflowTestRepository = repository;
        _professionalService = professionalService;
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

    public async Task<IPagedData<BackflowTestDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);
        query.Sort = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var tests = await _backflowTestRepository.SearchForProfessionalAsync(professional!.Id, pageInfo, query, cancellationToken);
        return tests.Select(m => MapToDto(m)!).ToPagedData(pageInfo);
    }
}