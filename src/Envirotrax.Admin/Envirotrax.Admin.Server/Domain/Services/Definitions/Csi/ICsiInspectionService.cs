
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionService
{
    Task<IPagedData<CsiInspectionDto>> SearchAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    Task<CsiInspectionDetailsDto?> GetAsync(int id, CancellationToken cancellationToken);

    Task<CsiInspectionDetailsDto?> UpdateAsync(int id, CsiInspectionUpdateRequest request, CancellationToken cancellationToken);

    Task<CsiInspectionCountsDto?> GetCountsAsync(int id, CancellationToken cancellationToken);

    Task<List<CsiInspectionAssemblyDto>?> GetAssembliesAsync(int id, CancellationToken cancellationToken);

    Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken);

    Task<List<CsiInspectionImageDto>?> GetImagesAsync(int id, CancellationToken cancellationToken);

    Task<CsiInspectionImageDto?> AddImageAsync(int id, Stream fileStream, string fileName, string? description, CancellationToken cancellationToken);

    Task DeleteImageAsync(int id, int imageId, CancellationToken cancellationToken);
}
