
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalSupplierRepository : IRepository<ProfessionalWaterSupplier>
{
    Task<IEnumerable<AvailableWaterSupplier>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalWaterSupplier>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalWaterSupplier>> GetCsiSuppliersForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
}