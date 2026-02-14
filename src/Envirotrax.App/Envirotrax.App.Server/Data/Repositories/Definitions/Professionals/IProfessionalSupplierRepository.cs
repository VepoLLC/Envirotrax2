
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalSupplierRepository
{
    Task<IEnumerable<AvailableWaterSupplier>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalWaterSupplier>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProfessionalWaterSupplier> AddOrUpdateAsync(ProfessionalWaterSupplier profesisonalSupplier);
}