
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalSupplierService : IService<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>
{
    Task<IPagedData<AvailableWaterSupplierDto>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IPagedData<ProfessionalWaterSupplierDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalWaterSupplierDto>> GetCsiSuppliersForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
}