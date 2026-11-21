using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTesterGaugeService
{
    // With a userId, only the gauges assigned to that user of the company.
    Task<IPagedData<BackflowGaugeDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, int? userId = null);

    Task<BackflowGaugeDto?> AddAsync(int professionalId, BackflowGaugeDto gauge, Stream fileStream, string fileName, CancellationToken cancellationToken);

    Task<BackflowGaugeDto?> UpdateAsync(int professionalId, int gaugeId, BackflowGaugeDto gauge, CancellationToken cancellationToken);

    Task<string?> GetFileUrlAsync(int professionalId, int gaugeId, CancellationToken cancellationToken);

    Task DeleteAsync(int professionalId, int gaugeId, CancellationToken cancellationToken);
}
