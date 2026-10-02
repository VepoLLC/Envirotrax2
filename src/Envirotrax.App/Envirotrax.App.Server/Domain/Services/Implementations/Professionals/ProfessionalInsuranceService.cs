
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalInsuranceService : Service<ProfessionalInsurance, ProfessionalInsuranceDto>, IProfessionalInsuranceService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".pdf"];

    private static readonly InsuranceCheckResult[] PolicyResultPriority =
    [
        InsuranceCheckResult.Valid,
        InsuranceCheckResult.InvalidCoverage,
        InsuranceCheckResult.Unverified,
        InsuranceCheckResult.Expired
    ];

    private readonly IProfessionalInsuranceRepository _insuranceRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;
    private readonly IGeneralSettingsService _generalSettingsService;

    public ProfessionalInsuranceService(
        IMapper mapper,
        IProfessionalInsuranceRepository repository,
        IFileStorageService fileStorageService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService,
        IGeneralSettingsService generalSettingsService)
        : base(mapper, repository)
    {
        _insuranceRepository = repository;
        _fileStorageService = fileStorageService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
        _generalSettingsService = generalSettingsService;
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

    public async Task<IPagedData<WaterSupplierInsuranceDto>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalInsurance, WaterSupplierInsuranceDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalInsurance, WaterSupplierInsuranceDto>(Mapper);

        var items = (await _insuranceRepository.GetUnverifiedByWaterSupplierAsync(pageInfo, query, cancellationToken)).ToList();
        var professionalIds = items.Select(i => i.ProfessionalId).Distinct().ToList();

        var professionalTypes = await _insuranceRepository.GetProfessionalTypesAsync(professionalIds, cancellationToken);
        var professionalUsers = (await _insuranceRepository.GetProfessionalUsersAsync(professionalIds, cancellationToken)).ToList();

        var byAccount = professionalUsers.ToDictionary(pu => (pu.ProfessionalId, pu.UserId));
        var admins = professionalUsers
            .Where(pu => pu.IsAdmin)
            .GroupBy(pu => pu.ProfessionalId)
            .ToDictionary(group => group.Key, group => group.First());

        var dtos = items.Select(i =>
        {
            var submitter = ResolveSubmitter(i, byAccount, admins);

            return new WaterSupplierInsuranceDto
            {
                Id = i.Id,
                ProfessionalId = i.ProfessionalId,
                SubmittedOn = i.CreatedTime,
                UserEmail = i.CreatedBy?.Email,
                CompanyName = i.Professional?.Name,
                ContactName = submitter?.ContactName,
                InsuranceNumber = i.InsuranceNumber,
                ExpirationDate = i.ExpirationDate,
                ProfessionalType = professionalTypes.GetValueOrDefault(i.ProfessionalId)
            };
        });

        return dtos.ToPagedData(pageInfo);
    }

    private static ProfessionalUser? ResolveSubmitter(
        ProfessionalInsurance insurance,
        IReadOnlyDictionary<(int ProfessionalId, int UserId), ProfessionalUser> byAccount,
        IReadOnlyDictionary<int, ProfessionalUser> admins)
    {
        if (insurance.CreatedById != null && byAccount.TryGetValue((insurance.ProfessionalId, insurance.CreatedById.Value), out var submitter))
        {
            return submitter;
        }

        return admins.GetValueOrDefault(insurance.ProfessionalId);
    }

    public async Task<InsuranceCheckDto> CheckForWaterSupplierAsync(int professionalId, int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var settings = await _generalSettingsService.GetAsync(waterSupplierId, cancellationToken);
        var (isRequired, requiredAmount) = GetInsuranceRequirement(settings, professionalType);

        if (!isRequired)
        {
            return CreateInsuranceCheck(InsuranceCheckResult.NotRequired, requiredAmount);
        }

        var insurances = await GetAllByProfessionalIdsAsync([professionalId], cancellationToken);

        var policyResults = insurances[professionalId]
            .Select(insurance => GetPolicyResult(insurance, requiredAmount))
            .ToList();

        return CreateInsuranceCheck(SummarizePolicyResults(policyResults), requiredAmount);
    }

    public async Task EnsureSatisfiedForWaterSupplierAsync(int professionalId, int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var check = await CheckForWaterSupplierAsync(professionalId, waterSupplierId, professionalType, cancellationToken);

        if (!check.IsSatisfied)
        {
            throw new AppValidationException(check.Message!);
        }
    }

    private static (bool IsRequired, decimal Amount) GetInsuranceRequirement(GeneralSettingsDto? settings, ProfessionalType professionalType)
    {
        if (settings == null)
        {
            return (false, 0);
        }

        return professionalType switch
        {
            ProfessionalType.Bpat => (settings.BpatsRequireInsurance, settings.BpatsRequireInsuranceAmount),
            ProfessionalType.CsiInspector => (settings.CsiInspectorsRequireInsurance, settings.CsiInspectorsRequireInsuranceAmount),
            ProfessionalType.FogTransporter => (settings.FogTransportersRequireInsurance, settings.FogTransportersRequireInsuranceAmount),
            _ => (false, 0)
        };
    }

    private static InsuranceCheckResult GetPolicyResult(ProfessionalInsuranceDto insurance, decimal requiredAmount)
    {
        if (insurance.ExpirationDate == null)
        {
            return InsuranceCheckResult.Unverified;
        }

        if (insurance.ExpirationType == ExpirationType.Expired)
        {
            return InsuranceCheckResult.Expired;
        }

        return (insurance.CoverageAmount ?? 0) >= requiredAmount
            ? InsuranceCheckResult.Valid
            : InsuranceCheckResult.InvalidCoverage;
    }

    private static InsuranceCheckResult SummarizePolicyResults(IReadOnlyCollection<InsuranceCheckResult> policyResults)
    {
        foreach (var result in PolicyResultPriority)
        {
            if (policyResults.Contains(result))
            {
                return result;
            }
        }

        return InsuranceCheckResult.NotFound;
    }

    private static InsuranceCheckDto CreateInsuranceCheck(InsuranceCheckResult result, decimal requiredAmount)
    {
        return new InsuranceCheckDto
        {
            Result = result,
            RequiredAmount = requiredAmount,
            Message = GetInsuranceCheckMessage(result, requiredAmount)
        };
    }

    private static string? GetInsuranceCheckMessage(InsuranceCheckResult result, decimal requiredAmount)
    {
        return result switch
        {
            InsuranceCheckResult.NotFound => "No insurance policy found",
            InsuranceCheckResult.Unverified => "Insurance policy awaiting validation...",
            InsuranceCheckResult.Expired => "Expired insurance policy",
            InsuranceCheckResult.InvalidCoverage => $"Requires ${requiredAmount.ToString("#,0", CultureInfo.InvariantCulture)} in insurance coverage",
            InsuranceCheckResult.Valid => "Insurance policy valid",
            _ => null
        };
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
}
