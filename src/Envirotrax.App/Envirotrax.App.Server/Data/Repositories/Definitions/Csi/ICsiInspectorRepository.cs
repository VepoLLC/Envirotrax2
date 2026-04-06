using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi
{
    public interface ICsiInspectorRepository : IRepository<Professional>
    {
        Task<Professional?> GetWithStateAsync(int id, CancellationToken cancellationToken);
        Task<List<ProfessionalWaterSupplier>> GetWaterSuppliersAsync(int professionalId, CancellationToken cancellationToken);
        Task<List<ProfessionalUser>> GetSubAccountsAsync(int professionalId, CancellationToken cancellationToken);
        Task<List<ProfessionalUserLicense>> GetLicensesAsync(int professionalId, CancellationToken cancellationToken);
    }
}
