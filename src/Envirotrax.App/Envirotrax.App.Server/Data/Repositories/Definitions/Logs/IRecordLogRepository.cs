using Envirotrax.App.Server.Data.Models.Logs;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Logs;

public interface IRecordLogRepository : IRepository<RecordLog>
{
    Task<List<RecordLog>> GetByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
    Task<int> GetCountByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
}
