
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface IProfessionalUserLicenseService : IService<ProfessionalUserLicense, ProfessionalUserLicenseDto>
{
    Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int userId, PageInfo pageInfo, Query query);
    Task<IPagedData<ProfessionalUserLicenseDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUserLicense, bool>>? filter = null);
    Task<ProfessionalUserLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto);
    Task<ProfessionalUserLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto);
    Task<IPagedData<WaterSupplierLicenseDto>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? licenseFilter, CancellationToken cancellationToken);
    Task<LicenseCountsDto> GetCountsByWaterSupplierAsync(CancellationToken cancellationToken);
    Task<WaterSupplierLicenseDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierLicenseDto dto, CancellationToken cancellationToken);
    Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken);
}
