using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTestService
{
    Task<IPagedData<BackflowTestDto>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken);

    Task<BackflowTestDetailsDto?> GetAsync(int id, CancellationToken cancellationToken);

    Task<BackflowTestDetailsDto?> UpdateAsync(int id, int waterSupplierId, BackflowTestUpdateRequest request, CancellationToken cancellationToken);

    Task<BackflowTestDetailsDto?> UploadImageAsync(int id, int waterSupplierId, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken);

    Task<BackflowTestCountsDto?> GetCountsAsync(int id, CancellationToken cancellationToken);

    Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken);

    Task<List<SiteLogDto>?> GetSiteLogsAsync(int id, CancellationToken cancellationToken);
}
