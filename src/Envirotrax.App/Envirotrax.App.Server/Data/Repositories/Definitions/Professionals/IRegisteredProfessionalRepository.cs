
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

/// <summary>
/// Read-only access to the publicly listed professionals of a water supplier.
/// Deliberately not an <see cref="IRepository{TModel}"/> — the only consumer is an anonymous
/// endpoint, so no write operation is exposed.
/// </summary>
public interface IRegisteredProfessionalRepository
{
    Task<IEnumerable<RegisteredProfessionalSupplier>> GetWaterSuppliersAsync(
        ProfessionalType professionalType,
        CancellationToken cancellationToken);

    Task<IEnumerable<RegisteredProfessional>> SearchAsync(
        int waterSupplierId,
        ProfessionalType professionalType,
        PageInfo pageInfo,
        Query query,
        CancellationToken cancellationToken);
}
