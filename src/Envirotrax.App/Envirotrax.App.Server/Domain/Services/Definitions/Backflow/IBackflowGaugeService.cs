
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowGaugeService : IService<BackflowGaugeDto>
{
    Task<BackflowGaugeDto> AddWithFileAsync(Stream fileStream, string originalFileName, BackflowGaugeDto dto);
    Task<IPagedData<BackflowGaugeDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<IPagedData<BackflowGaugeDto>> GetAllByProfessionalUserAsync(int professionalId, int userId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<Uri?> GenerateFileUrlAsync(int id, CancellationToken cancellationToken);
    Task<IPagedData<BackflowGaugeDto>> GetUsableByBpatAsync(int bpatUserId, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<BackflowGaugeDto> GetValidGaugeForBpatAsync(int gaugeId, int bpatUserId, CancellationToken cancellationToken);
    Task<int> CountUsableByCurrentUserAsync(CancellationToken cancellationToken);
    Task DeleteAssignedToUserAsync(int professionalId, int userId, CancellationToken cancellationToken);

    Task<IPagedData<WaterSupplierGaugeDto>> GetUnverifiedAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    Task<int> CountUnverifiedAsync(CancellationToken cancellationToken);
}
