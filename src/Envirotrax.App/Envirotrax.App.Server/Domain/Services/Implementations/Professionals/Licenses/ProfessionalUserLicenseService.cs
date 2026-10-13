
using System.Linq.Expressions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class ProfessionalUserLicenseService : Service<ProfessionalUserLicense, ProfessionalUserLicenseDto>, IProfessionalUserLicenseService
{
    private const string FireLicenseLabel = "Fire License";
    private const string DefaultLicenseLabel = "BPAT License";

    private static readonly LicenseCheckResult[] LicenseResultPriority =
    [
        LicenseCheckResult.Valid,
        LicenseCheckResult.Unverified,
        LicenseCheckResult.Expired
    ];

    private readonly IProfessionalUserLicenseRepository _licenseRepository;
    private readonly IProfessionalLicenseRepository _companyLicenseRepository;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;
    private readonly IRecordLogService _recordLogService;

    public ProfessionalUserLicenseService(
        IMapper mapper,
        IProfessionalUserLicenseRepository repository,
        IProfessionalLicenseRepository companyLicenseRepository,
        IProfessionalLicenseTypeService licenseTypeService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _licenseRepository = repository;
        _companyLicenseRepository = companyLicenseRepository;
        _licenseTypeService = licenseTypeService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
        _recordLogService = recordLogService;
    }

    protected override ProfessionalUserLicenseDto? MapToDto(ProfessionalUserLicense? model)
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

    public async Task<IPagedData<ProfessionalUserLicenseDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUserLicense, bool>>? filter = null)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);

        var items = await _licenseRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, filter);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ILookup<int, ProfessionalUserLicenseDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var items = await _licenseRepository.GetAllByProfessionalIdsAsync(professionalIds, professionalType, cancellationToken);
        return items.ToLookup(l => l.ProfessionalId, l => MapToDto(l)!);
    }

    public override async Task<ProfessionalUserLicenseDto> AddAsync(ProfessionalUserLicenseDto dto)
    {
        var licenseType = await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.User);

        dto.ProfessionalType = licenseType.ProfessionalType;
        dto.ExpirationDate = null;

        return await base.AddAsync(dto);
    }

    public override async Task<ProfessionalUserLicenseDto> UpdateAsync(ProfessionalUserLicenseDto dto)
    {
        var licenseType = await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.User);
        var existing = await Repository.GetNoIncludesAsync(dto.Id, default);

        if (existing == null)
        {
            throw new InvalidOperationException($"License {dto.Id} not found.");
        }

        dto.ProfessionalType = licenseType.ProfessionalType;
        dto.ExpirationDate = existing.ExpirationDate;

        if (existing.LicenseNumber != dto.LicenseNumber || existing.LicenseTypeId != licenseType.Id)
        {
            dto.ExpirationDate = null;
        }

        return await base.UpdateAsync(dto);
    }

    public async Task<ProfessionalUserLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto)
    {
        var licenseType = await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.User);

        dto.ProfessionalType = licenseType.ProfessionalType;

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;
        var added = await Repository.AddAsync(model);
        return MapToDto(added)!;
    }

    public async Task<ProfessionalUserLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto)
    {
        var licenseType = await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.User);

        dto.ProfessionalType = licenseType.ProfessionalType;

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;
        var updated = await Repository.UpdateAsync(model);
        return MapToDto(updated)!;
    }

    public async Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int userId, PageInfo pageInfo, Query query)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);

        var licenses = await _licenseRepository.GetAllAsync(userId, pageInfo, query);
        var dtoList = licenses.Select(l => MapToDto(l)!);

        return dtoList.ToPagedData(pageInfo);
    }

    public async Task<WaterSupplierLicenseDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierLicenseDto dto, CancellationToken cancellationToken)
    {
        var saved = await _licenseRepository.UpdateForWaterSupplierAsync(id, dto.LicenseNumber, dto.ContactName, dto.ExpirationDate, cancellationToken);
        var license = saved!;

        var now = _timeZoneHelper.GetUserLocalTime();
        return MapToWaterSupplierDto(license, now);
    }

    public async Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.DeleteForWaterSupplierAsync(id, cancellationToken);

        // recordLog manual
        await _recordLogService.AddAsync(RecordLogTableNames.ProfessionalUserLicenses, license.Id, _authService.WaterSupplierId, RecordLogType.Delete,
            $"Deleted license — LicenseNumber: '{license.LicenseNumber}', ExpirationDate: '{license.ExpirationDate:d}'", professionalId: license.ProfessionalId);
    }

    public async Task<BpatLicenseCheckDto> CheckBpatForWaterSupplierAsync(int professionalId, int userId, int waterSupplierId, CancellationToken cancellationToken)
    {
        var licenseTypes = await _licenseRepository.GetLicenseTypesForWaterSupplierAsync(waterSupplierId, ProfessionalType.Bpat, cancellationToken);
        var userLicenses = await _licenseRepository.GetUserLicensesForWaterSupplierAsync(professionalId, userId, waterSupplierId, ProfessionalType.Bpat, cancellationToken);
        var companyLicenses = await _companyLicenseRepository.GetForWaterSupplierAsync(professionalId, waterSupplierId, ProfessionalType.Bpat, cancellationToken);

        var licenseTypesByFire = licenseTypes.ToLookup(licenseType => licenseType.IsFireLicense);
        var bpatLicenses = userLicenses.Where(license => !license.LicenseType!.IsFireLicense);
        var fireLicenses = companyLicenses.Where(license => license.LicenseType!.IsFireLicense);

        var mainLicenseLabel = licenseTypesByFire[false].FirstOrDefault()?.Description ?? DefaultLicenseLabel;

        return new BpatLicenseCheckDto
        {
            License = CheckLicense(licenseTypesByFire[false].Any(), bpatLicenses, mainLicenseLabel),
            FireLicense = CheckLicense(licenseTypesByFire[true].Any(), fireLicenses, FireLicenseLabel)
        };
    }

    private LicenseCheckDto CheckLicense(bool isRequired, IEnumerable<ILicenseModel> licenses, string label)
    {
        if (!isRequired)
        {
            return CreateLicenseCheck(LicenseCheckResult.NotRequired, label, null);
        }

        var now = _timeZoneHelper.GetUserLocalTime();

        var candidates = licenses
            .Select(license => (License: license, Result: GetLicenseResult(license, now)))
            .ToList();

        foreach (var result in LicenseResultPriority)
        {
            var match = candidates.FirstOrDefault(candidate => candidate.Result == result);

            if (match.License != null)
            {
                return CreateLicenseCheck(result, label, match.License);
            }
        }

        return CreateLicenseCheck(LicenseCheckResult.NotFound, label, null);
    }

    private static LicenseCheckResult GetLicenseResult(ILicenseModel license, DateTime now)
    {
        if (license.ExpirationDate == null)
        {
            return LicenseCheckResult.Unverified;
        }

        return now > license.ExpirationDate
            ? LicenseCheckResult.Expired
            : LicenseCheckResult.Valid;
    }

    private static LicenseCheckDto CreateLicenseCheck(LicenseCheckResult result, string label, ILicenseModel? license)
    {
        return new LicenseCheckDto
        {
            Result = result,
            Label = label,
            Message = GetLicenseCheckMessage(result),
            LicenseNumber = license?.LicenseNumber,
            LicenseTypeName = license?.LicenseType?.Name,
            ExpirationDate = license?.ExpirationDate
        };
    }

    private static string GetLicenseCheckMessage(LicenseCheckResult result)
    {
        return result switch
        {
            LicenseCheckResult.NotRequired => "Not Required",
            LicenseCheckResult.Unverified => "License awaiting validation...",
            LicenseCheckResult.Expired => "License expired",
            LicenseCheckResult.Valid => "License valid",
            _ => "No license found"
        };
    }

    private static WaterSupplierLicenseDto MapToWaterSupplierDto(ProfessionalUserLicense license, DateTime now)
    {
        return new WaterSupplierLicenseDto
        {
            Id = license.Id,
            LicenseScope = LicenseScope.User,
            ProfessionalId = license.ProfessionalId,
            UserId = license.UserId,
            SubmittedOn = license.CreatedTime,
            UserEmail = license.User?.Email,
            CompanyName = license.Professional?.Name,
            ContactName = license.ProfessionalUser?.ContactName,
            ProfessionalType = license.ProfessionalType,
            LicenseTypeId = license.LicenseTypeId,
            LicenseTypeName = license.LicenseType?.Name,
            LicenseNumber = license.LicenseNumber,
            ExpirationDate = license.ExpirationDate,
            ExpirationType = license.ExpirationDate.HasValue
                ? (license.ExpirationDate < now ? ExpirationType.Expired
                    : license.ExpirationDate < now.AddDays(30) ? ExpirationType.AboutToExpire
                    : ExpirationType.Valid)
                : ExpirationType.Valid
        };
    }
}
