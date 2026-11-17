using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Logs;

public interface IRecordLogService
{
    Task<IPagedData<RecordLogDto>> SearchAsync(RecordLogSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
