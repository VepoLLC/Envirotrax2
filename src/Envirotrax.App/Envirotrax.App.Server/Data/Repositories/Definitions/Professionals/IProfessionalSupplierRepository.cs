
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalSupplierRepository
{
    Task<IEnumerable<ProfessionalWaterSupplier>> GetAllAsync(CancellationToken cancellationToken);
    Task<ProfessionalWaterSupplier> AddOrUpdateAsync(ProfessionalWaterSupplier profesisonalSupplier);
}