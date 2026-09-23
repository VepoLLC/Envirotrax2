using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Logs;

public interface IRecordLogService
{
    Task<List<RecordLogDto>> GetByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
    Task<int> GetCountByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken);
    Task AddAsync(string tableName, int recordId, int? waterSupplierId, RecordLogType logType, string? description, int? professionalId = null);
}
