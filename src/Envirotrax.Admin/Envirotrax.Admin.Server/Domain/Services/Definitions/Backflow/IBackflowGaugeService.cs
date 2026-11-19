using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowGaugeService
{
    Task<IPagedData<WaterSupplierGaugeDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<BackflowGaugeDto?> UpdateAsync(int id, BackflowGaugeDto gauge);

    Task<string?> GetFileUrlAsync(int id, CancellationToken cancellationToken);
}
