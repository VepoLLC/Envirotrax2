using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogTransporterLicenseService
{
    Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken);

    Task<IPagedData<ProfessionalLicenseDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalLicenseDto?> AddAsync(int professionalId, ProfessionalLicenseDto license, CancellationToken cancellationToken);

    Task<ProfessionalLicenseDto?> UpdateAsync(int professionalId, int licenseId, ProfessionalLicenseDto license, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken);
}
