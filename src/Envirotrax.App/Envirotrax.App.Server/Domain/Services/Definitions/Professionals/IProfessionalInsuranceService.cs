
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IProfessionalInsuranceService : IService<ProfessionalInsuranceDto>
{
    Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto insurance);
    Task<IPagedData<ProfessionalInsuranceDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<ILookup<int, ProfessionalInsuranceDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken);
    Task<IPagedData<WaterSupplierInsuranceDto>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<InsuranceCheckDto> CheckForWaterSupplierAsync(int professionalId, int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);

    Task EnsureSatisfiedForWaterSupplierAsync(int professionalId, int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);

    Task<Uri?> GenerateFileUrlAsync(int id, CancellationToken cancellationToken);
}