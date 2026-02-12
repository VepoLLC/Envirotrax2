
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalSupplierService
{
    Task<IEnumerable<ProfessionalWaterSupplierDto>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProfessionalWaterSupplierDto> AddOrUpdateAsync(ProfessionalWaterSupplierDto proSupplier);
}