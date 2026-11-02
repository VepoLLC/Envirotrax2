
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalInsuranceRepository : IRepository<ProfessionalInsurance>
{
    Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
    Task<ProfessionalInsurance?> GetCurrentForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
    Task<ProfessionalInsurance?> GetForProfessionalAsync(int professionalId, int insuranceId, CancellationToken cancellationToken);
    Task<bool> ExistsForProfessionalAsync(int professionalId, int insuranceId, CancellationToken cancellationToken);

    Task<IEnumerable<ProfessionalInsurance>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<Dictionary<int, ProfessionalType?>> GetProfessionalTypesAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
}