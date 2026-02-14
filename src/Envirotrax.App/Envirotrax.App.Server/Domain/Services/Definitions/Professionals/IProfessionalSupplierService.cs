
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalSupplierService
{
    Task<IPagedData<AvailableWaterSupplierDto>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalWaterSupplierDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProfessionalWaterSupplierDto> AddOrUpdateAsync(ProfessionalWaterSupplierDto proSupplier);
}