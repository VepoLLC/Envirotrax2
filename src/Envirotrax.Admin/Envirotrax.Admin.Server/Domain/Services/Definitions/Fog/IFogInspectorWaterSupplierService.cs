using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectorWaterSupplierService
{
    Task<IPagedData<ProfessionalWaterSupplierDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalWaterSupplierDto?> UpdateAsync(int professionalId, int waterSupplierId, ProfessionalWaterSupplierDto registration, CancellationToken cancellationToken);
}
