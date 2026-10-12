
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
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

    Task<PublicBackflowTestDetailsDto?> GetBackflowTestAsync(int id, CancellationToken cancellationToken);

    Task<byte[]?> GetBackflowTestPdfAsync(int id, CancellationToken cancellationToken);

    Task<PublicCsiInspectionDetailsDto?> GetCsiInspectionAsync(int id, CancellationToken cancellationToken);

    Task<List<CsiInspectionAssemblyDto>> GetCsiInspectionAssembliesAsync(int id, CancellationToken cancellationToken);

    Task<List<CsiInspectionImageDto>> GetCsiInspectionImagesAsync(int id, CancellationToken cancellationToken);

    Task<byte[]?> GetCsiInspectionPdfAsync(int id, CancellationToken cancellationToken);
}
