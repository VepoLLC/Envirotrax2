using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteLogService : ISiteLogService
{
    private static readonly string[] AllowedFileExtensions =
        [".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff", ".gif", ".pdf", ".xlsx", ".xls", ".txt", ".doc", ".docx"];

    private readonly IMapper _mapper;
    private readonly ISiteLogRepository _repository;
    private readonly IFileStorageService _fileStorageService;

    public SiteLogService(IMapper mapper, ISiteLogRepository repository, IFileStorageService fileStorageService)
    {
        _mapper = mapper;
        _repository = repository;
        _fileStorageService = fileStorageService;
    }

    public async Task<IPagedData<SiteLogDto>> GetBySiteAsync(int siteId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<SiteLog, SiteLogDto>(_mapper);
        query.Sort = query.ConvertSortProperties<SiteLog, SiteLogDto>(_mapper);

        var results = await _repository.GetBySiteAsync(siteId, pageInfo, query, cancellationToken);
        var dtos = results.Select(MapToDto).ToList();

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<IPagedData<PropertyLogDto>> GetForManagementAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<SiteLog, PropertyLogDto>(_mapper);
        query.Sort = query.ConvertSortProperties<SiteLog, PropertyLogDto>(_mapper);

        var results = await _repository.GetAllAsync(pageInfo, query, cancellationToken);
        var dtos = results.Select(MapToPropertyLogDto).ToList();

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<IEnumerable<SiteLogDto>> GetBySitesAsync(IEnumerable<int> siteIds, CancellationToken cancellationToken)
    {
        var results = await _repository.GetBySiteIdsAsync(siteIds, cancellationToken);

        return results
            .GroupBy(sl => sl.SiteId)
            .SelectMany(group => group.Take(5))
            .Select(MapToDto)
            .ToList();
    }

    public async Task<string?> GetAttachmentUrlAsync(int logId, CancellationToken cancellationToken)
    {
        var log = await _repository.GetAsync(logId, cancellationToken);

        if (log?.FileAttachmentPath == null || log.SkipFile)
        {
            return null;
        }

        return (await _fileStorageService.GenerateSasUrlAsync(log.FileAttachmentPath)).ToString();
    }

    private SiteLogDto MapToDto(SiteLog model)
    {
        var dto = _mapper.Map<SiteLogDto>(model)!;
        dto.ReviewDateStatus = ComputeReviewDateStatus(dto.LogType, dto.ReviewDate, DateTime.UtcNow);

        return dto;
    }

    private PropertyLogDto MapToPropertyLogDto(SiteLog model)
    {
        var dto = _mapper.Map<PropertyLogDto>(model)!;
        dto.ReviewDateStatus = ComputeReviewDateStatus(dto.LogType, dto.ReviewDate, DateTime.UtcNow);

        return dto;
    }

    public async Task<SiteLogDto> AddAsync(int siteId, SiteLogDto dto, Stream? fileStream, string? fileName)
    {
        dto.Site = new ReferencedSiteDto { Id = siteId };

        var entity = _mapper.Map<SiteLog>(dto);
        entity.SiteId = siteId;

        string? filePath = null;
        if (fileStream != null && fileName != null)
        {
            var ext = ValidateAndGetExtension(fileName);
            filePath = $"site-logs/{siteId}/{Guid.NewGuid()}{ext}";
            entity.FileAttachmentName = Path.GetFileName(fileName);
            entity.FileAttachmentPath = filePath;
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var saved = await _repository.AddAsync(entity);
        if (filePath != null && fileStream != null)
        {
            await _fileStorageService.UploadAsync(filePath, fileStream);
        }
        scope.Complete();

        return MapToDto(saved);
    }

    public async Task<SiteLogDto?> UpdateAsync(SiteLogDto dto, Stream? fileStream, string? fileName)
    {
        var existing = await _repository.GetAsync(dto.Id, CancellationToken.None);
        if (existing == null) return null;

        existing.LogType = dto.LogType;
        existing.NoteText = dto.NoteText;
        existing.ReviewDate = dto.ReviewDate;
        existing.AssemblyId = dto.Assembly?.Id;
        existing.SkipFile = dto.SkipFile;

        string? filePath = null;
        if (fileStream != null && fileName != null)
        {
            ValidateAndGetExtension(fileName);
            // Reuse the existing blob path so UploadAsync overwrites in place — no delete + new GUID needed
            filePath = existing.FileAttachmentPath ?? $"site-logs/{existing.SiteId}/{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            existing.FileAttachmentName = Path.GetFileName(fileName);
            existing.FileAttachmentPath = filePath;
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var updated = await _repository.UpdateAsync(existing);
        if (filePath != null && fileStream != null)
        {
            await _fileStorageService.UploadAsync(filePath, fileStream);
        }
        scope.Complete();

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _repository.GetAsync(id, CancellationToken.None);

        if (entity == null)
        {
            return false;
        }

        var filePath = entity.FileAttachmentPath;

        await _repository.DeleteAsync(id);

        if (filePath != null && !entity.SkipFile)
        {
            await _fileStorageService.DeleteAsync(filePath);
        }

        return true;
    }

    private static SiteLogReviewDateStatus ComputeReviewDateStatus(SiteLogType logType, DateTime? reviewDate, DateTime now)
    {
        if (!reviewDate.HasValue)
        {
            return SiteLogReviewDateStatus.None;
        }

        if (logType == SiteLogType.CompletedReminder)
        {
            return SiteLogReviewDateStatus.Completed;
        }

        if (reviewDate.Value < now)
        {
            return SiteLogReviewDateStatus.Overdue;
        }

        if (reviewDate.Value <= now.AddDays(30))
        {
            return SiteLogReviewDateStatus.DueSoon;
        }

        return SiteLogReviewDateStatus.Upcoming;
    }

    private static string ValidateAndGetExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        if (!AllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");
        }
        return ext;
    }
}
