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
        var dtos = results.Select(m => _mapper.Map<SiteLogDto>(m)!).ToList();

        var now = DateTime.UtcNow;

        foreach (var dto in dtos)
        {
            dto.ReviewDateStatus = ComputeReviewDateStatus(dto, now);
        }

        if (dtos.Any(d => d.FileAttachmentPath != null && !d.SkipFile))
        {
            var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();
            foreach (var dto in dtos.Where(d => d.FileAttachmentPath != null && !d.SkipFile))
            {
                dto.Url = (await _fileStorageService.GenerateSasUrlAsync(delegationKey, dto.FileAttachmentPath!)).ToString();
            }
        }

        return dtos.ToPagedData(pageInfo);
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

        var result = _mapper.Map<SiteLogDto>(saved);
        result.ReviewDateStatus = ComputeReviewDateStatus(result, DateTime.UtcNow);
        if (result.FileAttachmentPath != null && !result.SkipFile)
        {
            result.Url = (await _fileStorageService.GenerateSasUrlAsync(result.FileAttachmentPath)).ToString();
        }
        return result;
    }

    public async Task<SiteLogDto?> UpdateAsync(SiteLogDto dto, Stream? fileStream, string? fileName, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetAsync(dto.Id, cancellationToken);
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

        var result = _mapper.Map<SiteLogDto>(updated);
        result.ReviewDateStatus = ComputeReviewDateStatus(result, DateTime.UtcNow);
        if (result.FileAttachmentPath != null && !result.SkipFile)
        {
            result.Url = (await _fileStorageService.GenerateSasUrlAsync(result.FileAttachmentPath)).ToString();
        }
        return result;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(id, cancellationToken);

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

    private static SiteLogReviewDateStatus ComputeReviewDateStatus(SiteLogDto dto, DateTime now)
    {
        if (!dto.ReviewDate.HasValue)
        {
            return SiteLogReviewDateStatus.None;
        }

        if (dto.LogType == SiteLogType.CompletedReminder)
        {
            return SiteLogReviewDateStatus.Completed;
        }

        if (dto.ReviewDate.Value < now)
        {
            return SiteLogReviewDateStatus.Overdue;
        }

        if (dto.ReviewDate.Value <= now.AddDays(30))
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
