using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface IWaterSupplierLicenseService
{
    Task<IPagedData<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<LicenseCountsDto> GetCountsAsync(CancellationToken cancellationToken);
    Task<IPagedData<WaterSupplierLicenseDto>> GetUnverifiedRegistrationsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> CountUnverifiedRegistrationsAsync(CancellationToken cancellationToken);
}
