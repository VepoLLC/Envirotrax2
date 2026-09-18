
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalInsuranceService : Service<ProfessionalInsurance, ProfessionalInsuranceDto>, IProfessionalInsuranceService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".pdf"];

    private readonly IProfessionalInsuranceRepository _insuranceRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;

    public ProfessionalInsuranceService(
        IMapper mapper,
        IProfessionalInsuranceRepository repository,
        IFileStorageService fileStorageService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService)
        : base(mapper, repository)
    {
        _insuranceRepository = repository;
        _fileStorageService = fileStorageService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
    }

    protected override ProfessionalInsuranceDto? MapToDto(ProfessionalInsurance? model)
    {
        var dto = base.MapToDto(model);

        if (dto != null)
        {
            var localTime = _timeZoneHelper.GetUserLocalTime();

            if (localTime > dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.Expired;
            }
            else if (localTime.AddDays(30) >= dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.AboutToExpire;
            }
        }

        return dto;
    }

    public async Task<IPagedData<ProfessionalInsuranceDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalInsurance, ProfessionalInsuranceDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalInsurance, ProfessionalInsuranceDto>(Mapper);

        var items = await _insuranceRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ILookup<int, ProfessionalInsuranceDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        var items = await _insuranceRepository.GetAllByProfessionalIdsAsync(professionalIds, cancellationToken);
        return items.ToLookup(i => i.ProfessionalId, i => MapToDto(i)!);
    }

    public async Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto dto)
    {
        var fileExtension = Path.GetExtension(originalFileName);

        if (!AllowedFileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");
        }

        var professionalId = dto.Professional?.Id ?? _authService.ProfessionalId;

        dto.FilePath = $"professionals/{professionalId}/insurances/{Guid.NewGuid()}{fileExtension}";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await AddAsync(dto);
        await _fileStorageService.UploadAsync(dto.FilePath, fileStream);
        scope.Complete();

        return added;
    }

    /// <summary>
    /// Lets a contractor correct only the policy number they typed in themselves. The expiration date
    /// and the coverage amount are transcribed off the certificate by water supplier staff, so a
    /// contractor writing them would be self-validating and would walk straight past the review queue.
    /// The inherited CRUD update binds the whole DTO, which is why it is deliberately bypassed here.
    /// </summary>
    public async Task<ProfessionalInsuranceDto?> UpdateForProfessionalAsync(ProfessionalInsuranceDto insurance, CancellationToken cancellationToken)
    {
        // The ProfessionalId query filter scopes this to the caller, so it doubles as the ownership check.
        var existing = await _insuranceRepository.GetTrackedForUpdateAsync(insurance.Id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        existing.InsuranceNumber = insurance.InsuranceNumber;

        await _insuranceRepository.SaveChangesAsync();

        return MapToDto(existing);
    }

    public override async Task<ProfessionalInsuranceDto?> DeleteAsync(int id)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var deleted = await base.DeleteAsync(id);

            if (deleted != null)
            {
                await _fileStorageService.DeleteAsync(deleted.FilePath!);
                scope.Complete();
            }

            return deleted;
        }
    }

    public async Task<Uri?> GenerateFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetNoIncludesAsync(id, cancellationToken);

        if (insurance != null && !string.IsNullOrWhiteSpace(insurance.FilePath))
        {
            return await _fileStorageService.GenerateSasUrlAsync(insurance.FilePath);
        }

        return null;
    }

    public async Task<IPagedData<WaterSupplierInsuranceDto>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? insuranceFilter, CancellationToken cancellationToken)
    {
        var items = (await _insuranceRepository.GetAllByWaterSupplierAsync(pageInfo, query, insuranceFilter, cancellationToken)).ToList();
        var now = _timeZoneHelper.GetUserLocalTime();

        var professionalTypes = await _insuranceRepository.GetProfessionalTypesAsync(
            items.Select(i => i.ProfessionalId).Distinct(),
            cancellationToken);

        return items.Select(i => MapToWaterSupplierDto(i, now, professionalTypes.GetValueOrDefault(i.ProfessionalId))).ToPagedData(pageInfo);
    }

    public async Task<InsuranceCountsDto> GetCountsByWaterSupplierAsync(CancellationToken cancellationToken)
    {
        var unverified = await _insuranceRepository.GetCountByWaterSupplierAsync("unverified", cancellationToken);
        var expired = await _insuranceRepository.GetCountByWaterSupplierAsync("expired", cancellationToken);
        var expiring = await _insuranceRepository.GetCountByWaterSupplierAsync("expiring", cancellationToken);

        return new InsuranceCountsDto
        {
            UnverifiedCount = unverified,
            ExpiredCount = expired,
            ExpiringCount = expiring
        };
    }

    public async Task<WaterSupplierInsuranceDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierInsuranceDto insurance, CancellationToken cancellationToken)
    {
        var updated = await _insuranceRepository.UpdateForWaterSupplierAsync(
            id,
            insurance.InsuranceNumber,
            insurance.ExpirationDate,
            insurance.InsuranceCoverage,
            cancellationToken);

        var professionalTypes = await _insuranceRepository.GetProfessionalTypesAsync([updated.ProfessionalId], cancellationToken);

        return MapToWaterSupplierDto(updated, _timeZoneHelper.GetUserLocalTime(), professionalTypes.GetValueOrDefault(updated.ProfessionalId));
    }

    public async Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        // Throws when the policy belongs to a professional this water supplier is not registered with.
        await _insuranceRepository.GetForWaterSupplierAsync(id, cancellationToken);

        // Reused so the certificate file is removed in the same transaction as the row.
        await DeleteAsync(id);
    }

    public async Task<Uri?> GenerateFileUrlForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        var insurance = await _insuranceRepository.GetForWaterSupplierAsync(id, cancellationToken);

        if (string.IsNullOrWhiteSpace(insurance.FilePath))
        {
            return null;
        }

        return await _fileStorageService.GenerateSasUrlAsync(insurance.FilePath);
    }

    private static WaterSupplierInsuranceDto MapToWaterSupplierDto(ProfessionalInsurance insurance, DateTime now, ProfessionalType? professionalType)
    {
        return new WaterSupplierInsuranceDto
        {
            Id = insurance.Id,
            ProfessionalId = insurance.ProfessionalId,
            CompanyName = insurance.Professional?.Name,
            CompanyEmail = insurance.Professional?.CompanyEmail,
            InsuranceNumber = insurance.InsuranceNumber,
            InsuranceCoverage = insurance.InsuranceCoverage,
            ExpirationDate = insurance.ExpirationDate,
            ExpirationType = insurance.ExpirationDate.HasValue
                ? (insurance.ExpirationDate < now ? ExpirationType.Expired
                    : insurance.ExpirationDate < now.AddDays(30) ? ExpirationType.AboutToExpire
                    : ExpirationType.Valid)
                : ExpirationType.Valid,
            ProfessionalType = professionalType
        };
    }
}
