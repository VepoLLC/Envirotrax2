
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalInsuranceService : IService<ProfessionalInsuranceDto>
{
    Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto insurance);
    Task<ProfessionalInsuranceDto?> UpdateForProfessionalAsync(ProfessionalInsuranceDto insurance, CancellationToken cancellationToken);
    Task<IPagedData<ProfessionalInsuranceDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ILookup<int, ProfessionalInsuranceDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);

    Task<Uri?> GenerateFileUrlAsync(int id, CancellationToken cancellationToken);

    Task<IPagedData<WaterSupplierInsuranceDto>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? insuranceFilter, CancellationToken cancellationToken);
    Task<InsuranceCountsDto> GetCountsByWaterSupplierAsync(CancellationToken cancellationToken);
    Task<WaterSupplierInsuranceDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierInsuranceDto insurance, CancellationToken cancellationToken);
    Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken);
    Task<Uri?> GenerateFileUrlForWaterSupplierAsync(int id, CancellationToken cancellationToken);
}