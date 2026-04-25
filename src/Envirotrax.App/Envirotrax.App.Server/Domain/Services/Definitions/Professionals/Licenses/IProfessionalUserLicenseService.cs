
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface IProfessionalUserLicenseService : IService<ProfessionalUserLicense, ProfessionalUserLicenseDto>
{
    Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int userId, PageInfo pageInfo, Query query);
    Task<IPagedData<ProfessionalUserLicenseDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ProfessionalUserLicenseDto?> GetCsiLicenseForUserAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<ProfessionalUserLicenseDto>> GetCsiLicensesForProfessionalAsync(int professionalId, CancellationToken cancellationToken);
    Task<ProfessionalUserLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto);
    Task<ProfessionalUserLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto);
}
