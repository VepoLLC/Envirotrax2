
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.PublicSearch;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.PublicSearch;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.PublicSearch;

public class PublicSearchRepository : Repository<WaterSupplier, int, PublicDbContext>, IPublicSearchRepository
{
    public PublicSearchRepository(PublicDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<PublicSearchWaterSupplier>> GetWaterSuppliersAsync(string? domain, CancellationToken cancellationToken)
    {
        return await GetEligibleSuppliersQuery()
            .WhereIf(domain != null, supplier => supplier.Domain == domain)
            .OrderBy(supplier => supplier.Name)
            .Select(supplier => new PublicSearchWaterSupplier
            {
                Id = supplier.Id,
                Name = supplier.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PublicBackflowTestResult>> SearchBackflowTestsAsync(
        PublicSearchCriteria criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var tests = RestrictToSupplier(DbContext.BackflowTests.AsNoTracking(), criteria.WaterSupplierId)
            .Where(test => test.DeletedTime == null
                && test.IsCurrent
                && !test.OutOfService
                && test.TransactionId != null
                && test.TransactionId != string.Empty);

        tests = ApplyCriteria(tests, criteria);

        var results = tests
            .Select(test => new PublicBackflowTestResult
            {
                Id = test.Id,
                TestDate = test.TestDate,
                ExpirationDate = test.ExpirationDate,
                RenewalRequired = test.RenewalRequired,
                Manufacturer = test.Manufacturer,
                Model = test.Model,
                Size = test.Size,
                DeviceType = test.DeviceType,
                SerialNumber = test.SerialNumber,
                HazardType = test.HazardType,
                HazardTypeOtherDescription = test.HazardTypeOtherDescription,
                PropertyBusinessName = test.PropertyBusinessName,
                PropertyStreetNumber = test.PropertyStreetNumber,
                PropertyStreetName = test.PropertyStreetName,
                PropertyNumber = test.PropertyNumber,
                PropertyCity = test.PropertyCity,
                PropertyState = test.PropertyState != null ? test.PropertyState.Code : null,
                PropertyZip = test.PropertyZip
            })
            .OrderByDescending(result => result.TestDate)
            .ThenByDescending(result => result.Id);

        var paginated = await results.PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PublicCsiInspectionResult>> SearchCsiInspectionsAsync(
        PublicSearchCriteria criteria,
        PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var inspections = RestrictToSupplier(DbContext.CsiInspections.AsNoTracking(), criteria.WaterSupplierId)
            .Where(inspection => inspection.DeletedTime == null
                && inspection.TransactionId != null
                && inspection.TransactionId != string.Empty);

        inspections = ApplyCriteria(inspections, criteria);

        var results = inspections
            .Select(inspection => new PublicCsiInspectionResult
            {
                Id = inspection.Id,
                InspectionDate = inspection.InspectionDate,
                PropertyBusinessName = inspection.PropertyBusinessName,
                PropertyStreetNumber = inspection.PropertyStreetNumber,
                PropertyStreetName = inspection.PropertyStreetName,
                PropertyNumber = inspection.PropertyNumber,
                PropertyCity = inspection.PropertyCity,
                PropertyState = inspection.PropertyState != null ? inspection.PropertyState.Code : null,
                PropertyZip = inspection.PropertyZip,
                InspectorCompanyName = inspection.InspectorCompanyName,
                InspectorContactName = inspection.InspectorContactName,
                InspectorAddress = inspection.InspectorAddress,
                InspectorCity = inspection.InspectorCity,
                InspectorState = inspection.InspectorState,
                InspectorZip = inspection.InspectorZip
            })
            .OrderByDescending(result => result.InspectionDate)
            .ThenByDescending(result => result.Id);

        var paginated = await results.PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<BackflowTest?> GetBackflowTestAsync(int id, CancellationToken cancellationToken)
    {
        return await RestrictToEligibleSupplier(DbContext.BackflowTests.AsNoTracking())
            .Include(test => test.WaterSupplier)
                .ThenInclude(supplier => supplier!.State)
            .Include(test => test.Site)
            .Include(test => test.Professional)
            .Include(test => test.Bpat)
                .ThenInclude(bpat => bpat!.User)
            .Include(test => test.BpatState)
            .Include(test => test.PropertyState)
            .Include(test => test.MailingState)
            .Where(test => test.Id == id
                && test.DeletedTime == null
                && test.TransactionId != null
                && test.TransactionId != string.Empty)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BackflowSettings?> GetBackflowSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await DbContext.BackflowSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(settings => settings.WaterSupplierId == waterSupplierId, cancellationToken);
    }

    public async Task<CsiInspection?> GetCsiInspectionAsync(int id, CancellationToken cancellationToken)
    {
        return await RestrictToEligibleSupplier(DbContext.CsiInspections.AsNoTracking())
            .Include(inspection => inspection.Site)
            .Include(inspection => inspection.WaterSupplier)
                .ThenInclude(supplier => supplier!.State)
            .Include(inspection => inspection.Professional)
            .Include(inspection => inspection.Inspector)
                .ThenInclude(inspector => inspector!.User)
            .Include(inspection => inspection.PropertyState)
            .Include(inspection => inspection.MailingState)
            .Where(inspection => inspection.Id == id
                && inspection.DeletedTime == null
                && inspection.TransactionId != null
                && inspection.TransactionId != string.Empty)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetCsiInspectionAssembliesAsync(
        int waterSupplierId,
        int inspectionId,
        CancellationToken cancellationToken)
    {
        return await DbContext.CsiInspectionVisuallyIdentifiedAssemblies
            .AsNoTracking()
            .Include(assembly => assembly.Test)
            .Where(assembly => assembly.WaterSupplierId == waterSupplierId && assembly.InspectionId == inspectionId)
            .OrderBy(assembly => assembly.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CsiInspectionImage>> GetCsiInspectionImagesAsync(
        int waterSupplierId,
        int inspectionId,
        CancellationToken cancellationToken)
    {
        return await DbContext.CsiInspectionImages
            .AsNoTracking()
            .Where(image => image.WaterSupplierId == waterSupplierId && image.InspectionId == inspectionId)
            .OrderBy(image => image.Id)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<WaterSupplier> GetEligibleSuppliersQuery()
    {
        return GetListQuery()
            .Where(supplier => supplier.IsActive
                && supplier.DeletedTime == null
                && supplier.GeneralSettings != null
                && supplier.GeneralSettings.BackflowTesting
                && !supplier.GeneralSettings.AdministrativeOnly
                && !supplier.GeneralSettings.PrivacyRequired);
    }

    private IQueryable<TRecord> RestrictToSupplier<TRecord>(IQueryable<TRecord> records, int waterSupplierId)
        where TRecord : TenantModel<WaterSupplier>
    {
        var eligibleSuppliers = GetEligibleSuppliersQuery();

        return RestrictToEligibleSupplier(records).Where(record =>
            (record.WaterSupplierId == waterSupplierId || record.WaterSupplier!.ParentId == waterSupplierId)
            && eligibleSuppliers.Any(supplier => supplier.Id == waterSupplierId));
    }

    private IQueryable<TRecord> RestrictToEligibleSupplier<TRecord>(IQueryable<TRecord> records)
        where TRecord : TenantModel<WaterSupplier>
    {
        var eligibleSuppliers = GetEligibleSuppliersQuery();

        return records.Where(record => eligibleSuppliers.Any(supplier => supplier.Id == record.WaterSupplierId));
    }

    private static IQueryable<BackflowTest> ApplyCriteria(IQueryable<BackflowTest> tests, PublicSearchCriteria criteria)
    {
        if (criteria.PropertyBusinessName is { Length: > 0 } businessName)
        {
            tests = tests.Where(test => test.PropertyBusinessName!.Contains(businessName));
        }

        if (criteria.PropertyStreetNumber is { Length: > 0 } streetNumber)
        {
            tests = tests.Where(test => test.PropertyStreetNumber!.Contains(streetNumber));
        }

        if (criteria.PropertyStreetName is { Length: > 0 } streetName)
        {
            tests = tests.Where(test => test.PropertyStreetName!.Contains(streetName));
        }

        if (criteria.PropertyNumber is { Length: > 0 } number)
        {
            tests = tests.Where(test => test.PropertyNumber!.Contains(number));
        }

        return tests;
    }

    private static IQueryable<CsiInspection> ApplyCriteria(IQueryable<CsiInspection> inspections, PublicSearchCriteria criteria)
    {
        if (criteria.PropertyBusinessName is { Length: > 0 } businessName)
        {
            inspections = inspections.Where(inspection => inspection.PropertyBusinessName!.Contains(businessName));
        }

        if (criteria.PropertyStreetNumber is { Length: > 0 } streetNumber)
        {
            inspections = inspections.Where(inspection => inspection.PropertyStreetNumber!.Contains(streetNumber));
        }

        if (criteria.PropertyStreetName is { Length: > 0 } streetName)
        {
            inspections = inspections.Where(inspection => inspection.PropertyStreetName!.Contains(streetName));
        }

        if (criteria.PropertyNumber is { Length: > 0 } number)
        {
            inspections = inspections.Where(inspection => inspection.PropertyNumber!.Contains(number));
        }

        return inspections;
    }
}
