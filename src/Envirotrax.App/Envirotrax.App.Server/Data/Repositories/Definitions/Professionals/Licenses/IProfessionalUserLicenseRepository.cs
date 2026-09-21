
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;

public interface IProfessionalUserLicenseRepository : IRepository<ProfessionalUserLicense>
{
    Task<IEnumerable<ProfessionalUserLicense>> GetAllAsync(int userId, PageInfo pageInfo, Query query);
    Task<IEnumerable<ProfessionalUserLicense>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUserLicense, bool>>? filter = null);
    Task<IEnumerable<ProfessionalUserLicense>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUserLicense>> GetBpatLicensesForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUserLicense>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? licenseFilter, CancellationToken cancellationToken);
    Task<int> GetCountByWaterSupplierAsync(string? licenseFilter, CancellationToken cancellationToken);
    Task<ProfessionalUserLicense> UpdateForWaterSupplierAsync(int id, string licenseNumber, string? contactName, DateTime? expirationDate, CancellationToken cancellationToken);
    Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken);
}
