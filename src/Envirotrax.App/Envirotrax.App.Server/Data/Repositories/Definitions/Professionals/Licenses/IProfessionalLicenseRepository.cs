using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;

public interface IProfessionalLicenseRepository : IRepository<ProfessionalLicense>
{
    Task<IEnumerable<ProfessionalLicense>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalLicense>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken);
    Task<ProfessionalLicense> UpdateForWaterSupplierAsync(int id, string licenseNumber, DateTime? expirationDate);
    Task<ProfessionalLicense> DeleteForWaterSupplierAsync(int id);
}
