
using System.Linq.Expressions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class ProfessionalUserLicenseService : Service<ProfessionalUserLicense, ProfessionalUserLicenseDto>, IProfessionalUserLicenseService
{
    private readonly IProfessionalUserLicenseRepository _licenseRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public ProfessionalUserLicenseService(
        IMapper mapper,
        IProfessionalUserLicenseRepository repository,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _licenseRepository = repository;
        _timeZoneHelper = timeZoneHelper;
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

    public async Task<ProfessionalUserLicenseDto> AddForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto)
    {
        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;
        var added = await Repository.AddAsync(model);
        return MapToDto(added)!;
    }

    public async Task<ProfessionalUserLicenseDto> UpdateForProfessionalAsync(int professionalId, ProfessionalUserLicenseDto dto)
    {
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

    public async Task<IPagedData<WaterSupplierLicenseDto>> GetAllByWaterSupplierAsync(PageInfo pageInfo, Query query, string? licenseFilter, CancellationToken cancellationToken)
    {
        var items = await _licenseRepository.GetAllByWaterSupplierAsync(pageInfo, query, licenseFilter, cancellationToken);
        var now = _timeZoneHelper.GetUserLocalTime();

        var dtos = items.Select(l => new WaterSupplierLicenseDto
        {
            Id = l.Id,
            ProfessionalId = l.ProfessionalId,
            UserId = l.UserId,
            UserEmail = l.User?.Email,
            CompanyName = l.Professional?.Name,
            ContactName = l.ProfessionalUser?.ContactName,
            ProfessionalType = l.ProfessionalType,
            LicenseTypeId = l.LicenseTypeId,
            LicenseTypeName = l.LicenseType?.Name,
            LicenseNumber = l.LicenseNumber,
            ExpirationDate = l.ExpirationDate,
            ExpirationType = l.ExpirationDate.HasValue
                ? (l.ExpirationDate < now ? ExpirationType.Expired
                    : l.ExpirationDate < now.AddDays(30) ? ExpirationType.AboutToExpire
                    : ExpirationType.Valid)
                : ExpirationType.Valid
        });

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<LicenseCountsDto> GetCountsByWaterSupplierAsync(CancellationToken cancellationToken)
    {
        var unverified = await _licenseRepository.GetCountByWaterSupplierAsync("unverified", cancellationToken);
        var expired = await _licenseRepository.GetCountByWaterSupplierAsync("expired", cancellationToken);
        var expiring = await _licenseRepository.GetCountByWaterSupplierAsync("expiring", cancellationToken);

        return new LicenseCountsDto
        {
            UnverifiedCount = unverified,
            ExpiredCount = expired,
            ExpiringCount = expiring
        };
    }

    public async Task<WaterSupplierLicenseDto> UpdateForWaterSupplierAsync(int id, UpdateWaterSupplierLicenseDto dto, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.UpdateForWaterSupplierAsync(id, dto.LicenseNumber, dto.ContactName, dto.ExpirationDate, cancellationToken);
        var now = _timeZoneHelper.GetUserLocalTime();
        return new WaterSupplierLicenseDto
        {
            Id = license.Id,
            ProfessionalId = license.ProfessionalId,
            UserId = license.UserId,
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

    public async Task DeleteForWaterSupplierAsync(int id, CancellationToken cancellationToken)
    {
        await _licenseRepository.DeleteForWaterSupplierAsync(id, cancellationToken);
    }
}
