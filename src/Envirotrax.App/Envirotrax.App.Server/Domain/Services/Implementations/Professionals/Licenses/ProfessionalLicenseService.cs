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

public class ProfessionalLicenseService : Service<ProfessionalLicense, ProfessionalLicenseDto>, IProfessionalLicenseService
{
    private readonly IProfessionalLicenseRepository _licenseRepository;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IAuthService _authService;
    private readonly IRecordLogService _recordLogService;

    public ProfessionalLicenseService(
        IMapper mapper,
        IProfessionalLicenseRepository repository,
        IProfessionalLicenseTypeService licenseTypeService,
        ITimeZoneHelperService timeZoneHelper,
        IAuthService authService,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _licenseRepository = repository;
        _licenseTypeService = licenseTypeService;
        _timeZoneHelper = timeZoneHelper;
        _authService = authService;
        _recordLogService = recordLogService;
    }

    protected override ProfessionalLicenseDto? MapToDto(ProfessionalLicense? model)
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

    public async Task<IPagedData<ProfessionalLicenseDto>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalLicense, ProfessionalLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalLicense, ProfessionalLicenseDto>(Mapper);

        var licenses = await _licenseRepository.GetAllByProfessionalAsync(professionalId, professionalType, pageInfo, query, cancellationToken);

        return licenses.Select(license => MapToDto(license)!).ToPagedData(pageInfo);
    }

    public async Task<ILookup<int, ProfessionalLicenseDto>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetAllByProfessionalIdsAsync(professionalIds, professionalType, cancellationToken);

        return licenses.ToLookup(license => license.ProfessionalId, license => MapToDto(license)!);
    }

    public override async Task<ProfessionalLicenseDto> AddAsync(ProfessionalLicenseDto dto)
    {
        await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.Company);

        return await base.AddAsync(dto);
    }

    public override async Task<ProfessionalLicenseDto> UpdateAsync(ProfessionalLicenseDto dto)
    {
        await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.Company);

        return await base.UpdateAsync(dto);
    }

    public async Task<ProfessionalLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalLicenseDto dto)
    {
        await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.Company);

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;

        var added = await Repository.AddAsync(model);

        return MapToDto(added)!;
    }

    public async Task<ProfessionalLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalLicenseDto dto)
    {
        await _licenseTypeService.EnsureLicenseScopeAsync(dto.LicenseType.Id, LicenseScope.Company);

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;

        var updated = await Repository.UpdateAsync(model);

        return MapToDto(updated)!;
    }

    public async Task<WaterSupplierLicenseDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierLicenseDto dto)
    {
        var license = await _licenseRepository.UpdateForWaterSupplierAsync(id, dto.LicenseNumber, dto.ExpirationDate);
        var now = _timeZoneHelper.GetUserLocalTime();

        return MapToWaterSupplierDto(license, now);
    }

    public async Task DeleteForWaterSupplierAsync(int id)
    {
        var license = await _licenseRepository.DeleteForWaterSupplierAsync(id);

        await _recordLogService.AddAsync(RecordLogTableNames.ProfessionalLicenses, license.Id, _authService.WaterSupplierId, RecordLogType.Delete,
            $"Deleted license — LicenseNumber: '{license.LicenseNumber}', ExpirationDate: '{license.ExpirationDate:d}'", professionalId: license.ProfessionalId);
    }

    private static WaterSupplierLicenseDto MapToWaterSupplierDto(ProfessionalLicense license, DateTime now)
    {
        var expirationType = ExpirationType.Valid;

        if (license.ExpirationDate < now)
        {
            expirationType = ExpirationType.Expired;
        }
        else if (license.ExpirationDate < now.AddDays(30))
        {
            expirationType = ExpirationType.AboutToExpire;
        }

        return new WaterSupplierLicenseDto
        {
            Id = license.Id,
            LicenseScope = LicenseScope.Company,
            ProfessionalId = license.ProfessionalId,
            SubmittedOn = license.CreatedTime,
            UserEmail = license.CreatedBy?.Email,
            CompanyName = license.Professional?.Name,
            ProfessionalType = license.ProfessionalType,
            LicenseTypeId = license.LicenseTypeId,
            LicenseTypeName = license.LicenseType?.Name,
            LicenseNumber = license.LicenseNumber,
            ExpirationDate = license.ExpirationDate,
            ExpirationType = expirationType
        };
    }
}
