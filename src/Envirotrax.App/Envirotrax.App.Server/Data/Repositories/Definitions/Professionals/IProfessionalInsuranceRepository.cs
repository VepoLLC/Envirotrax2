
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalInsuranceRepository : IRepository<ProfessionalInsurance>
{
    Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProfessionalInsurance>> GetAllForValidationAsync(int professionalId, CancellationToken cancellationToken);

    Task<IEnumerable<ProfessionalInsurance>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? insuranceFilter, CancellationToken cancellationToken);
    Task<int> GetCountByWaterSupplierAsync(string? insuranceFilter, CancellationToken cancellationToken);
    Task<ProfessionalInsurance> GetForWaterSupplierAsync(int id, CancellationToken cancellationToken);
    Task<ProfessionalInsurance> UpdateForWaterSupplierAsync(int id, string insuranceNumber, DateTime? expirationDate, decimal? insuranceCoverage, CancellationToken cancellationToken);
    Task<Dictionary<int, ProfessionalType?>> GetProfessionalTypesAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
}