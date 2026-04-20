
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
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
            else if (localTime.AddDays(-30) <= dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.AboutToExpire;
            }
        }

        return dto;
    }

    public async Task<IPagedData<ProfessionalUserLicenseDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);

        var items = await _licenseRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalUserLicenseDto?> GetCsiLicenseForUserAsync(int userId, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetCsiLicenseForUserAsync(userId, cancellationToken);
        return MapToDto(license);
    }

    public async Task<IEnumerable<ProfessionalUserLicenseDto>> GetCsiLicensesForProfessionalAsync(int professionalId, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetCsiLicensesForProfessionalAsync(professionalId, cancellationToken);
        return licenses.Select(l => MapToDto(l)!);
    }

    public async Task<IPagedData<ProfessionalUserLicenseDto>> GetAllAsync(int userId, PageInfo pageInfo, Query query)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUserLicense, ProfessionalUserLicenseDto>(Mapper);

        var licenses = await _licenseRepository.GetAllAsync(userId, pageInfo, query);
        var dtoList = licenses.Select(l => MapToDto(l)!);

        return dtoList.ToPagedData(pageInfo);
    }
}
