using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.Common;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals.Licenses;

public class WaterSupplierLicenseRepository : IWaterSupplierLicenseRepository
{
    private readonly TenantDbContext _dbContext;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly ITenantProvidersService _tenantProvider;

    public WaterSupplierLicenseRepository(IDbContextSelector dbContextSelector, ITimeZoneHelperService timeZoneHelper, ITenantProvidersService tenantProvider)
    {
        _dbContext = dbContextSelector.Current;
        _timeZoneHelper = timeZoneHelper;
        _tenantProvider = tenantProvider;
    }

    public Task<IEnumerable<WaterSupplierLicenseDto>> GetAllAsync(string? licenseFilter, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var licenses = ApplyLicenseFilter(GetLicensesQuery(), licenseFilter);

        return PaginateLicensesAsync(licenses, pageInfo, query, cancellationToken);
    }

    public Task<int> CountAsync(string? licenseFilter, CancellationToken cancellationToken)
    {
        return ApplyLicenseFilter(GetLicensesQuery(), licenseFilter).CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<WaterSupplierLicenseDto>> GetUnverifiedRegistrationsAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var registrations = ProjectCompanyLicenses(GetUnverifiedRegistrationsQuery());

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(WaterSupplierLicenseDto.Id)] = SortOperator.Asc;
        }

        var paginated = await registrations
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<int> CountUnverifiedRegistrationsAsync(CancellationToken cancellationToken)
    {
        return GetUnverifiedRegistrationsQuery().CountAsync(cancellationToken);
    }

    // A water supplier sees the licenses of professionals registered with it, without FOG transporter registrations
    // (those have their own Registrations page). An admin sees every license, registrations included, because the
    // admin window has no Registrations tab (and AdminDbContext has no tenant filters, so the registration check
    // would otherwise mean "registered with any supplier").
    private IQueryable<WaterSupplierLicenseDto> GetLicensesQuery()
    {
        var isWaterSupplier = !_tenantProvider.HasScope(ScopeDefinitions.AdminInternal);

        var userLicenses = _dbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .WhereIf(isWaterSupplier, license => license.ProfessionalType != ProfessionalType.FogTransporter
                && _dbContext.ProfessionalWaterSuppliers.Any(registration => registration.ProfessionalId == license.ProfessionalId));

        var companyLicenses = _dbContext.ProfessionalLicenses
            .AsNoTracking()
            .WhereIf(isWaterSupplier, license => license.ProfessionalType != ProfessionalType.FogTransporter
                && _dbContext.ProfessionalWaterSuppliers.Any(registration => registration.ProfessionalId == license.ProfessionalId));

        return ProjectUserLicenses(userLicenses).Concat(ProjectCompanyLicenses(companyLicenses));
    }

    private static async Task<IEnumerable<WaterSupplierLicenseDto>> PaginateLicensesAsync(IQueryable<WaterSupplierLicenseDto> licenses, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(WaterSupplierLicenseDto.SubmittedOn)] = SortOperator.Asc;
        }

        query.Sort[nameof(WaterSupplierLicenseDto.LicenseScope)] = SortOperator.Asc;
        query.Sort[nameof(WaterSupplierLicenseDto.Id)] = SortOperator.Asc;

        var paginated = await licenses
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    private IQueryable<ProfessionalLicense> GetUnverifiedRegistrationsQuery()
    {
        return _dbContext.ProfessionalLicenses
            .AsNoTracking()
            .Where(license => license.ProfessionalType == ProfessionalType.FogTransporter
                && license.ExpirationDate == null
                && _dbContext.ProfessionalWaterSuppliers.Any(registration => registration.ProfessionalId == license.ProfessionalId));
    }

    private static IQueryable<WaterSupplierLicenseDto> ProjectUserLicenses(IQueryable<ProfessionalUserLicense> licenses)
    {
        return licenses.Select(license => new WaterSupplierLicenseDto
        {
            Id = license.Id,
            LicenseScope = LicenseScope.User,
            ProfessionalId = license.ProfessionalId,
            UserId = license.UserId,
            SubmittedOn = license.CreatedTime,
            UserEmail = license.User!.Email,
            CompanyName = license.Professional!.Name,
            ContactName = license.ProfessionalUser!.ContactName,
            ProfessionalType = license.ProfessionalType,
            LicenseTypeId = license.LicenseTypeId,
            LicenseTypeName = license.LicenseType!.Name,
            LicenseNumber = license.LicenseNumber,
            ExpirationDate = license.ExpirationDate,
            LastCheckDate = license.LastCheckDate
        });
    }

    private IQueryable<WaterSupplierLicenseDto> ProjectCompanyLicenses(IQueryable<ProfessionalLicense> licenses)
    {
        return licenses.Select(license => new WaterSupplierLicenseDto
        {
            Id = license.Id,
            LicenseScope = LicenseScope.Company,
            ProfessionalId = license.ProfessionalId,
            UserId = license.CreatedById,
            SubmittedOn = license.CreatedTime,
            UserEmail = license.CreatedBy!.Email,
            CompanyName = license.Professional!.Name,
            ContactName = _dbContext.ProfessionalUsers
                .Where(professionalUser => professionalUser.ProfessionalId == license.ProfessionalId && professionalUser.UserId == license.CreatedById)
                .Select(professionalUser => professionalUser.ContactName)
                .FirstOrDefault(),
            ProfessionalType = license.ProfessionalType,
            LicenseTypeId = license.LicenseTypeId,
            LicenseTypeName = license.LicenseType!.Name,
            LicenseNumber = license.LicenseNumber,
            ExpirationDate = license.ExpirationDate,
            LastCheckDate = null
        });
    }

    private IQueryable<WaterSupplierLicenseDto> ApplyLicenseFilter(IQueryable<WaterSupplierLicenseDto> query, string? licenseFilter)
    {
        var now = _timeZoneHelper.GetUserLocalTime();
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

        return licenseFilter switch
        {
            "unverified" => query.Where(license => license.ExpirationDate == null),
            "expired" => query.Where(license => license.ExpirationDate != null
                && license.ExpirationDate >= firstDayLastMonth
                && license.ExpirationDate < firstDayThisMonth),
            "expiring" => query.Where(license => license.ExpirationDate != null
                && license.ExpirationDate >= firstDayThisMonth
                && license.ExpirationDate < firstDayThisMonth.AddMonths(1)),
            _ => query
        };
    }
}
