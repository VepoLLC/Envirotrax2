
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.PublicSearch;

/// <summary>
/// Read-only access for the anonymous Public Search page. Deliberately not an
/// <see cref="IRepository{TModel}"/> — nothing here may write.
/// </summary>
public interface IPublicSearchRepository
{
    Task<IEnumerable<PublicSearchWaterSupplier>> GetWaterSuppliersAsync(string? domain, CancellationToken cancellationToken);

    Task<IEnumerable<PublicBackflowTestResult>> SearchBackflowTestsAsync(
        PublicSearchCriteria criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken);

    Task<IEnumerable<PublicCsiInspectionResult>> SearchCsiInspectionsAsync(
        PublicSearchCriteria criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken);

    Task<BackflowTest?> GetBackflowTestAsync(int id, CancellationToken cancellationToken);

    Task<BackflowSettings?> GetBackflowSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<CsiInspection?> GetCsiInspectionAsync(int id, CancellationToken cancellationToken);

    Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetCsiInspectionAssembliesAsync(
        int waterSupplierId,
        int inspectionId,
        CancellationToken cancellationToken);

    Task<List<CsiInspectionImage>> GetCsiInspectionImagesAsync(
        int waterSupplierId,
        int inspectionId,
        CancellationToken cancellationToken);
}
