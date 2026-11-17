using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.Common.Data;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Logs;

public class RecordLogService : IRecordLogService
{
    private readonly IMapper _mapper;
    private readonly IRecordLogRepository _repository;
    private readonly ITenantProvidersService _tenantProvider;

    public RecordLogService(
        IMapper mapper,
        IRecordLogRepository repository,
        ITenantProvidersService tenantProvider)
    {
        _mapper = mapper;
        _repository = repository;
        _tenantProvider = tenantProvider;
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

    public async Task<IPagedData<RecordLogDto>> SearchAsync(RecordLogSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(criteria.TableName))
        {
            throw new AppValidationException("The Table Name field is required.");
        }

        query.Filter = query.ConvertFilterProperties<RecordLog, RecordLogDto>(_mapper);
        query.Sort = query.ConvertSortProperties<RecordLog, RecordLogDto>(_mapper);

        var logs = await _repository.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return logs
            .Select(log => _mapper.Map<RecordLogDto>(log))
            .ToPagedData(pageInfo);
    }

    public async Task AddAsync(string tableName, int recordId, int? waterSupplierId, RecordLogType logType, string? description, int? professionalId = null)
    {
        var log = new RecordLog
        {
            WaterSupplierId = waterSupplierId,
            ProfessionalId = professionalId,
            LogType = logType,
            TableName = tableName,
            RecordId = recordId,
            Description = description,
            IpAddress = _tenantProvider.IpAddress
        };

        await _repository.AddAsync(log);
    }
}
