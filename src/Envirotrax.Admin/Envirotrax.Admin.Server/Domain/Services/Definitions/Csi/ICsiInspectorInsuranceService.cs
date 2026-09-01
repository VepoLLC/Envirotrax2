using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorInsuranceService
{
    Task<IPagedData<ProfessionalInsuranceDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalInsuranceDto?> AddAsync(int professionalId, ProfessionalInsuranceDto insurance, Stream fileStream, string fileName, CancellationToken cancellationToken);

    Task<ProfessionalInsuranceDto?> UpdateAsync(int professionalId, int insuranceId, ProfessionalInsuranceDto insurance, CancellationToken cancellationToken);

    Task<string?> GetFileUrlAsync(int professionalId, int insuranceId, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int insuranceId, CancellationToken cancellationToken);
}
