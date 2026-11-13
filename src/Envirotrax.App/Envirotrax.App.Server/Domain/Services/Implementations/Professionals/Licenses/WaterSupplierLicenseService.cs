using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;

public class WaterSupplierLicenseService : IWaterSupplierLicenseService
{
    private readonly IWaterSupplierLicenseRepository _licenseRepository;
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly IBackflowGaugeService _gaugeService;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public WaterSupplierLicenseService(
        IWaterSupplierLicenseRepository licenseRepository,
        IProfessionalInsuranceService insuranceService,
        IBackflowGaugeService gaugeService,
        ITimeZoneHelperService timeZoneHelper)
    {
        _licenseRepository = licenseRepository;
        _insuranceService = insuranceService;
        _gaugeService = gaugeService;
        _timeZoneHelper = timeZoneHelper;
    }

    public async Task<IPagedData<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetAllAsync(licenseFilter, pageInfo, query, cancellationToken);

        SetExpirationTypes(licenses);
        SetHoursSinceLastCheck(licenses);

        return licenses.ToPagedData(pageInfo);
    }

    public async Task<LicenseCountsDto> GetCountsAsync(CancellationToken cancellationToken)
    {
        var unverified = await _licenseRepository.CountAsync("unverified", cancellationToken);
        var expired = await _licenseRepository.CountAsync("expired", cancellationToken);
        var expiring = await _licenseRepository.CountAsync("expiring", cancellationToken);

        return new LicenseCountsDto
        {
            UnverifiedCount = unverified,
            ExpiredCount = expired,
            ExpiringCount = expiring
        };
    }

    public async Task<LicenseAdminCountsDto> GetCountsForAdminAsync(CancellationToken cancellationToken)
    {
        var licenseCounts = await GetCountsAsync(cancellationToken);

        return new LicenseAdminCountsDto
        {
            UnverifiedCount = licenseCounts.UnverifiedCount,
            ExpiredCount = licenseCounts.ExpiredCount,
            ExpiringCount = licenseCounts.ExpiringCount,
            InsuranceCount = await _insuranceService.CountUnverifiedAsync(cancellationToken),
            GaugeCount = await _gaugeService.CountUnverifiedAsync(cancellationToken)
        };
    }

    public async Task<IPagedData<WaterSupplierLicenseDto>> GetUnverifiedRegistrationsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var registrations = await _licenseRepository.GetUnverifiedRegistrationsAsync(pageInfo, query, cancellationToken);

        SetExpirationTypes(registrations);

        return registrations.ToPagedData(pageInfo);
    }

    public Task<int> CountUnverifiedRegistrationsAsync(CancellationToken cancellationToken)
    {
        return _licenseRepository.CountUnverifiedRegistrationsAsync(cancellationToken);
    }

    private void SetExpirationTypes(IEnumerable<WaterSupplierLicenseDto> licenses)
    {
        var now = _timeZoneHelper.GetUserLocalTime();

        foreach (var license in licenses)
        {
            license.ExpirationType = ExpirationType.Valid;

            if (license.ExpirationDate < now)
            {
                license.ExpirationType = ExpirationType.Expired;
            }
            else if (license.ExpirationDate < now.AddDays(30))
            {
                license.ExpirationType = ExpirationType.AboutToExpire;
            }
        }
    }

    private static void SetHoursSinceLastCheck(IEnumerable<WaterSupplierLicenseDto> licenses)
    {
        var now = DateTime.UtcNow;

        foreach (var license in licenses)
        {
            if (license.LastCheckDate != null)
            {
                var sinceLastCheck = now - license.LastCheckDate.Value;

                license.HoursSinceLastCheck = (int)sinceLastCheck.TotalHours;
            }
        }
    }
}
