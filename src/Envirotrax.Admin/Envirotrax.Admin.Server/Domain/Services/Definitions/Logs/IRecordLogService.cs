using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Logs;

public interface IRecordLogService
{
    Task<IPagedData<RecordLogDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
