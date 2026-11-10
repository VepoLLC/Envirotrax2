using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;

public interface ILicenseService
{
    Task<IPagedData<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<LicenseAdminCountsDto?> GetCountsAsync(CancellationToken cancellationToken);

    Task<List<ProfessionalLicenseTypeDto>?> GetTypesAsync(CancellationToken cancellationToken);

    Task<ProfessionalUserLicenseDto?> UpdateAsync(int id, ProfessionalUserLicenseDto license);

    Task<TceqLicenseDto?> CheckTceqAsync(int id);

    Task<ProfessionalLicenseDto?> UpdateCompanyLicenseAsync(int id, ProfessionalLicenseDto license);
}
