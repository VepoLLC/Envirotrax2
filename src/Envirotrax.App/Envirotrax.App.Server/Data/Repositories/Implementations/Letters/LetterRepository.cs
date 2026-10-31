using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Letters;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Letters;

public class LetterRepository : ILetterRepository
{
    private const string AllValue = "All";
    private const int MaxUnknownAssemblyLetters = 2;

    private readonly TenantDbContext _dbContext;

    public LetterRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<List<LetterBackflowTestDto>> GetRenewalRequirementTestsAsync(int waterSupplierId, BackflowRenewalRequirementDto requirement, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken)
    {
        var propertyType = requirement.PropertyType;
        var deviceType = requirement.DeviceType;
        var hazardType = requirement.HazardType;

        var query = GetBackflowLetterTestsQuery(waterSupplierId, rangeStart, rangeEnd)
            .Where(test => test.Site!.PropertyType == propertyType);

        if (!IsAllValue(deviceType))
        {
            query = query.Where(test => test.DeviceType == deviceType);
        }

        if (!IsAllValue(hazardType))
        {
            query = query.Where(test => test.HazardType == hazardType);
        }

        if (requirement.HasSiteOssf)
        {
            query = query.Where(test => test.Site!.HasOnSiteSewageFacility);
        }

        if (requirement.AuxWaterSupply)
        {
            query = query.Where(test => test.Site!.HasAuxWaterSupply);
        }

        return await SelectLetterTests(query).ToListAsync(cancellationToken);
    }

    public async Task<List<LetterBackflowTestDto>> GetForcedRenewalTestsAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken)
    {
        var query = GetBackflowLetterTestsQuery(waterSupplierId, rangeStart, rangeEnd)
            .Where(test => test.ForceRenewal);

        return await SelectLetterTests(query).ToListAsync(cancellationToken);
    }

    public async Task<List<LetterSiteDto>> GetSitesAsync(List<int> siteIds, CancellationToken cancellationToken)
    {
        var query = _dbContext.Sites
            .Where(site => siteIds.Contains(site.Id));

        return await SelectLetterSites(query).ToListAsync(cancellationToken);
    }

    public async Task<List<LetterSiteDto>> GetUnknownAssembliesSitesAsync(int waterSupplierId, DateTime lastModifiedBefore, CancellationToken cancellationToken)
    {
        var query = GetWaterSupplierSitesQuery(waterSupplierId)
            .Where(site => site.HasKnownBackflowAssemblies && site.UnknownAssemblyLettersSent < MaxUnknownAssemblyLetters)
            .Where(site => (site.UpdatedTime != null && site.UpdatedTime < lastModifiedBefore)
                || (site.UpdatedTime == null && site.CreatedTime < lastModifiedBefore))
            .Where(site => site.Active && !site.OutOfArea && !site.InvalidMailingAddress)
            .Where(site => !_dbContext.BackflowTests.Any(test => test.SiteId == site.Id
                && test.DeletedTime == null
                && test.TransactionId != null
                && test.TransactionId != string.Empty
                && test.IsCurrent
                && !test.OutOfService))
            .OrderBy(site => site.Id);

        return await SelectLetterSites(query).ToListAsync(cancellationToken);
    }

    public async Task<List<LetterSiteDto>> GetCsiSitesAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken)
    {
        var query = GetWaterSupplierSitesQuery(waterSupplierId)
            .Where(site => site.NeedsCsiInspection)
            .Where(site => site.CsiRenewalDate >= rangeStart && site.CsiRenewalDate <= rangeEnd)
            .Where(site => site.Active && !site.OutOfArea && !site.InvalidMailingAddress && !site.ExcludeFromCsiMailing)
            .OrderByDescending(site => site.CsiRenewalDate);

        return await SelectLetterSites(query).ToListAsync(cancellationToken);
    }

    public async Task UpdateOptInCodeAsync(int siteId, string optInCodeHash, DateTime optInCodeExpirationDate)
    {
        await _dbContext.Sites
            .Where(site => site.Id == siteId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(site => site.OptInCodeHash, optInCodeHash)
                .SetProperty(site => site.OptInCodeExpirationDate, optInCodeExpirationDate));
    }

    public async Task IncrementUnknownAssemblyLettersSentAsync(List<int> siteIds)
    {
        await _dbContext.Sites
            .Where(site => siteIds.Contains(site.Id))
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(site => site.UnknownAssemblyLettersSent, site => site.UnknownAssemblyLettersSent + 1));
    }

    private IQueryable<BackflowTest> GetBackflowLetterTestsQuery(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd)
    {
        return _dbContext.BackflowTests
            .Where(test => test.WaterSupplierId == waterSupplierId || test.WaterSupplier!.ParentId == waterSupplierId)
            .Where(test => test.DeletedTime == null)
            .Where(test => test.SiteId != null && test.Site!.DeletedTime == null)
            .Where(test => test.ExpirationDate >= rangeStart && test.ExpirationDate <= rangeEnd)
            .Where(test => !test.OutOfService && test.IsCurrent)
            .Where(test => test.TransactionId != null && test.TransactionId != string.Empty)
            .Where(test => test.Site!.Active
                && !test.Site!.OutOfArea
                && !test.Site!.InvalidMailingAddress
                && !test.Site!.ExcludeFromBackflowMailing);
    }

    private IQueryable<Site> GetWaterSupplierSitesQuery(int waterSupplierId)
    {
        return _dbContext.Sites
            .Where(site => site.WaterSupplierId == waterSupplierId || site.WaterSupplier!.ParentId == waterSupplierId)
            .Where(site => site.DeletedTime == null);
    }

    private static IQueryable<LetterBackflowTestDto> SelectLetterTests(IQueryable<BackflowTest> query)
    {
        return query
            .OrderBy(test => test.ExpirationDate)
            .Select(test => new LetterBackflowTestDto
            {
                Id = test.Id,
                SiteId = test.SiteId!.Value,
                DeviceType = test.DeviceType,
                Manufacturer = test.Manufacturer,
                Model = test.Model,
                Size = test.Size,
                SerialNumber = test.SerialNumber,
                LocationDescription = test.LocationDescription,
                HazardType = test.HazardType,
                BpatCompanyName = test.BpatCompanyName,
                BpatWorkNumber = test.BpatWorkNumber,
                TransactionId = test.TransactionId,
                TestDate = test.TestDate,
                ExpirationDate = test.ExpirationDate
            });
    }

    private static IQueryable<LetterSiteDto> SelectLetterSites(IQueryable<Site> query)
    {
        return query.Select(site => new LetterSiteDto
        {
            Id = site.Id,
            AccountNumber = site.AccountNumber,
            WaterSupplierAccountNumber = site.WaterSupplierAccountNumber,
            PropertyType = site.PropertyType,
            BusinessName = site.BusinessName,
            StreetNumber = site.StreetNumber,
            StreetName = site.StreetName,
            PropertyNumber = site.PropertyNumber,
            City = site.City,
            StateCode = site.State!.Code,
            ZipCode = site.ZipCode,
            MailingCompanyName = site.MailingCompanyName,
            MailingContactName = site.MailingContactName,
            MailingStreetNumber = site.MailingStreetNumber,
            MailingStreetName = site.MailingStreetName,
            MailingNumber = site.MailingNumber,
            MailingCity = site.MailingCity,
            MailingStateCode = site.MailingState!.Code,
            MailingZipCode = site.MailingZipCode,
            MailingEmailAddress = site.MailingEmailAddress,
            RenewalOptInType = site.RenewalOptInType
        });
    }

    private static bool IsAllValue(string? value)
    {
        return string.IsNullOrEmpty(value) || string.Equals(value, AllValue, StringComparison.OrdinalIgnoreCase);
    }
}
