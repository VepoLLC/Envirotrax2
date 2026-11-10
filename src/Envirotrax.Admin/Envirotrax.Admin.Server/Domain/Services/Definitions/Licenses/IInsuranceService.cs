using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;

public interface IInsuranceService
{
    Task<IPagedData<WaterSupplierInsuranceDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalInsuranceDto?> UpdateAsync(int id, ProfessionalInsuranceDto insurance);

    Task<string?> GetFileUrlAsync(int id, CancellationToken cancellationToken);
}
