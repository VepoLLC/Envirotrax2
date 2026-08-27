using AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Logs;

public class RecordLogService : IRecordLogService
{
    private readonly IMapper _mapper;
    private readonly IRecordLogRepository _repository;
    private readonly IAuthService _authService;
    private readonly IHttpContextAccessor _contextAccessor;

    public RecordLogService(
        IMapper mapper,
        IRecordLogRepository repository,
        IAuthService authService,
        IHttpContextAccessor contextAccessor)
    {
        _mapper = mapper;
        _repository = repository;
        _authService = authService;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<RecordLogDto>> GetByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByRecordAsync(tableName, recordId, cancellationToken);

        return logs
            .Select(log => _mapper.Map<RecordLogDto>(log))
            .ToList();
    }

    public Task<int> GetCountByRecordAsync(string tableName, int recordId, CancellationToken cancellationToken)
    {
        return _repository.GetCountByRecordAsync(tableName, recordId, cancellationToken);
    }

    public async Task AddAsync(string tableName, int recordId, int waterSupplierId, RecordLogType logType, string? description)
    {
        var userId = _authService.UserId;

        var log = new RecordLog
        {
            WaterSupplierId = waterSupplierId,
            LogDate = DateTime.UtcNow,
            LogType = logType,
            TableName = tableName,
            RecordId = recordId,
            Description = description,
            UserId = userId > 0 ? userId : null,
            IpAddress = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        };

        await _repository.AddAsync(log);
    }
}
