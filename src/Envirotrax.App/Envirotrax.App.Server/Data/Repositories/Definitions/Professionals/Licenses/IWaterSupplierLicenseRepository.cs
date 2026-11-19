using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;

public interface IWaterSupplierLicenseRepository
{
    Task<IEnumerable<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> CountAsync(string? licenseFilter, CancellationToken cancellationToken);
    Task<IEnumerable<WaterSupplierLicenseDto>> GetUnverifiedRegistrationsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> CountUnverifiedRegistrationsAsync(CancellationToken cancellationToken);
}
