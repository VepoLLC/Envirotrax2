using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogInspectionService
{
    Task<IPagedData<FogInspectionDto>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<FogInspectionDto?> GetAsync(int id, CancellationToken cancellationToken);

    Task<FogInspectionDto?> UpdateAsync(int id, int waterSupplierId, FogInspectionUpdateRequest request, CancellationToken cancellationToken);

    Task<FogInspectionDto?> UploadImageAsync(int id, int waterSupplierId, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken);

    Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken);
}
