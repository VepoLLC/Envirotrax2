using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Logs;

public interface IRecordLogRepository : IRepository<RecordLog, long>
{
    Task<List<RecordLog>> GetByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
    Task<int> GetCountByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
    Task<IEnumerable<RecordLog>> SearchAsync(RecordLogSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
}
