
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Data.Repositories.Definitions.PublicSearch;
using Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;
using Envirotrax.App.Server.Domain.Services.Definitions.PublicSearch;

namespace Envirotrax.App.Server.Domain.Services.Implementations.PublicSearch;

public class PublicSearchService : IPublicSearchService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IMapper _mapper;
    private readonly IPublicSearchRepository _publicSearchRepository;

    public PublicSearchService(IMapper mapper, IPublicSearchRepository publicSearchRepository)
    {
        _mapper = mapper;
        _publicSearchRepository = publicSearchRepository;
    }

    public async Task<PublicSearchWaterSuppliersDto> GetWaterSuppliersAsync(string? domain, CancellationToken cancellationToken)
    {
        var suppliers = (await _publicSearchRepository.GetWaterSuppliersAsync(cancellationToken)).ToList();

        return new PublicSearchWaterSuppliersDto
        {
            Suppliers = _mapper.Map<List<PublicSearchWaterSupplierDto>>(suppliers),
            SelectedWaterSupplierId = FindSupplierIdByDomain(suppliers, domain)
        };
    }

    public async Task<IPagedData<PublicBackflowTestResultDto>> SearchBackflowTestsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var searchCriteria = BuildCriteria(criteria);

        if (searchCriteria == null)
        {
            return Enumerable.Empty<PublicBackflowTestResultDto>().ToPagedData(pageInfo);
        }

        LimitPageSize(pageInfo);

        var results = await _publicSearchRepository.SearchBackflowTestsAsync(searchCriteria, pageInfo, cancellationToken);

        return _mapper
            .Map<IEnumerable<PublicBackflowTestResult>, IEnumerable<PublicBackflowTestResultDto>>(results)
            .ToPagedData(pageInfo);
    }

    public async Task<IPagedData<PublicCsiInspectionResultDto>> SearchCsiInspectionsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var searchCriteria = BuildCriteria(criteria);

        if (searchCriteria == null)
        {
            return Enumerable.Empty<PublicCsiInspectionResultDto>().ToPagedData(pageInfo);
        }

        LimitPageSize(pageInfo);

        var results = await _publicSearchRepository.SearchCsiInspectionsAsync(searchCriteria, pageInfo, cancellationToken);

        return _mapper
            .Map<IEnumerable<PublicCsiInspectionResult>, IEnumerable<PublicCsiInspectionResultDto>>(results)
            .ToPagedData(pageInfo);
    }

    private PublicSearchCriteria? BuildCriteria(PublicSearchCriteriaDto dto)
    {
        var criteria = _mapper.Map<PublicSearchCriteria>(dto);

        criteria.PropertyBusinessName = NormalizeText(criteria.PropertyBusinessName);
        criteria.PropertyStreetNumber = NormalizeText(criteria.PropertyStreetNumber);
        criteria.PropertyStreetName = NormalizeText(criteria.PropertyStreetName);
        criteria.PropertyNumber = NormalizeText(criteria.PropertyNumber);

        var hasAddressCriteria = criteria.PropertyBusinessName != null
            || criteria.PropertyStreetNumber != null
            || criteria.PropertyStreetName != null
            || criteria.PropertyNumber != null;

        if (criteria.WaterSupplierId <= 0 || !hasAddressCriteria)
        {
            return null;
        }

        return criteria;
    }

    private static int? FindSupplierIdByDomain(IEnumerable<PublicSearchWaterSupplier> suppliers, string? domain)
    {
        var normalizedDomain = NormalizeText(domain);

        if (normalizedDomain == null)
        {
            return null;
        }

        return suppliers
            .Where(supplier => string.Equals(supplier.Domain.Trim(), normalizedDomain, StringComparison.OrdinalIgnoreCase))
            .OrderBy(supplier => supplier.Id)
            .Select(supplier => (int?)supplier.Id)
            .FirstOrDefault();
    }

    private static string? NormalizeText(string? value)
    {
        var trimmed = value?.Trim();

        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }

    private static void LimitPageSize(PageInfo pageInfo)
    {
        pageInfo.PageSize = pageInfo.PageSize > 0
            ? Math.Min(pageInfo.PageSize, MaxPageSize)
            : DefaultPageSize;
    }
}
