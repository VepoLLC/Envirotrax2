using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorLicenseService
{
    Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken);

    Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalUserLicenseDto?> AddAsync(int professionalId, ProfessionalUserLicenseDto license, CancellationToken cancellationToken);

    Task<ProfessionalUserLicenseDto?> UpdateAsync(int professionalId, int licenseId, ProfessionalUserLicenseDto license, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int licenseId, CancellationToken cancellationToken);
}
