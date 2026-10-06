
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;

namespace Envirotrax.App.Server.Domain.Services.Definitions.PublicSearch;

public interface IPublicSearchService
{
    Task<PublicSearchWaterSuppliersDto> GetWaterSuppliersAsync(string? domain, CancellationToken cancellationToken);

    Task<IPagedData<PublicBackflowTestResultDto>> SearchBackflowTestsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken);

    Task<IPagedData<PublicCsiInspectionResultDto>> SearchCsiInspectionsAsync(
        PublicSearchCriteriaDto criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken);
}
