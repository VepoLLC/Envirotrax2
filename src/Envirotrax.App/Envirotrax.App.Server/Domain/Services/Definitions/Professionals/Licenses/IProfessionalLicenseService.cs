using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

public interface IProfessionalLicenseService : IService<ProfessionalLicense, ProfessionalLicenseDto>
{
    Task<IPagedData<ProfessionalLicenseDto>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ILookup<int, ProfessionalLicenseDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken);
    Task<ProfessionalLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalLicenseDto dto);
    Task<ProfessionalLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalLicenseDto dto);
    Task<WaterSupplierLicenseDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierLicenseDto dto);
    Task DeleteForWaterSupplierAsync(int id);
}
