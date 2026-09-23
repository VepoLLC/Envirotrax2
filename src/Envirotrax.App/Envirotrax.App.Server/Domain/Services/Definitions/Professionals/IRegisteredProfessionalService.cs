
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IRegisteredProfessionalService
{
    Task<IReadOnlyList<RegisteredProfessionalSupplierDto>> GetWaterSuppliersAsync(
        ProfessionalType professionalType,
        CancellationToken cancellationToken);

    Task<IPagedData<RegisteredProfessionalDto>> SearchAsync(
        int waterSupplierId,
        ProfessionalType professionalType,
        PageInfo pageInfo,
        Query query,
        CancellationToken cancellationToken);
}
