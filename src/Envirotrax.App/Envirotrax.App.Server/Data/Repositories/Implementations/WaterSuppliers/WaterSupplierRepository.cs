
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class WaterSupplierRepository : Repository<WaterSupplier>, IWaterSupplierRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public WaterSupplierRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<WaterSupplier> GetListQuery()
    {
        return base.GetListQuery()
            .Include(supplier => supplier.Parent)
            .Include(supplier => supplier.State)
            .Include(supplier => supplier.GeneralSettings)
            .Where(supplier => supplier.DeletedTime == null)
            .WhereIf(!_tenantProvider.HasScope(ScopeDefinitions.AdminInternal), supplier => supplier.ParentId == _tenantProvider.WaterSupplierId)
            .AsNoTracking();
    }

    protected override IQueryable<WaterSupplier> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(supplier => supplier.Parent)
            .Include(supplier => supplier.State)
            .WhereIf(!_tenantProvider.HasScope(ScopeDefinitions.AdminInternal), supplier => supplier.ParentId == _tenantProvider.WaterSupplierId);
    }

    public async Task<IEnumerable<int>> GetSupplierIdsAsync(bool hasBackflowTests, CancellationToken cancellationToken)
    {
        return await DbContext
            .WaterSuppliers
            // Bypass the GeneralSettings tenant filter for cross-tenant supplier enumeration.
            .IgnoreQueryFilters()
            .Where(supplier => supplier.DeletedTime == null)
            .WhereIf(hasBackflowTests, supplier => supplier.IsActive
                && supplier.GeneralSettings != null
                && supplier.GeneralSettings.BackflowTesting
                && !supplier.GeneralSettings.AdministrativeOnly)
            .Select(supplier => supplier.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<WaterSupplier?> GetUnscopedAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await DbContext
            .WaterSuppliers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(supplier => supplier.DeletedTime == null)
            .FirstOrDefaultAsync(supplier => supplier.Id == waterSupplierId, cancellationToken);
    }

    public async Task<IEnumerable<int>> GetChildSupplierIdsAsync(int parentWaterSupplierId, CancellationToken cancellationToken)
    {
        return await DbContext
            .WaterSuppliers
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(supplier => supplier.DeletedTime == null)
            .Where(supplier => supplier.ParentId == parentWaterSupplierId)
            .Select(supplier => supplier.Id)
            .ToListAsync(cancellationToken);
    }

    public override Task<WaterSupplier> AddAsync(WaterSupplier supplier)
    {
        supplier.ParentId = _tenantProvider.WaterSupplierId;
        return base.AddAsync(supplier);
    }

    public override async Task<WaterSupplier?> UpdateAsync(WaterSupplier supplier)
    {
        var isAdmin = _tenantProvider.HasScope(ScopeDefinitions.AdminInternal);

        var dbSupplier = await DbContext.WaterSuppliers
            .WhereIf(!isAdmin, x => x.ParentId == _tenantProvider.WaterSupplierId)
            .SingleOrDefaultAsync(x => x.Id == supplier.Id);

        if (dbSupplier == null)
        {
            return null;
        }

        if (!isAdmin)
        {
            dbSupplier.ParentId = _tenantProvider.WaterSupplierId;
        }

        dbSupplier.Name = supplier.Name;
        dbSupplier.Domain = supplier.Domain;
        dbSupplier.IsActive = supplier.IsActive;
        dbSupplier.UpdatedTime = DateTime.UtcNow;
        dbSupplier.ContactName = supplier.ContactName;
        dbSupplier.PwsId = supplier.PwsId;
        dbSupplier.Address = supplier.Address;
        dbSupplier.City = supplier.City;
        dbSupplier.StateId = supplier.StateId;
        dbSupplier.ZipCode = supplier.ZipCode;
        dbSupplier.PhoneNumber = supplier.PhoneNumber;
        dbSupplier.FaxNumber = supplier.FaxNumber;
        dbSupplier.EmailAddress = supplier.EmailAddress;

        dbSupplier.LetterCompanyName = supplier.LetterCompanyName;
        dbSupplier.LetterContactName = supplier.LetterContactName;
        dbSupplier.LetterAddress = supplier.LetterAddress;
        dbSupplier.LetterCity = supplier.LetterCity;
        dbSupplier.LetterStateId = supplier.LetterStateId;
        dbSupplier.LetterZipCode = supplier.LetterZipCode;

        dbSupplier.LetterContactCompanyName = supplier.LetterContactCompanyName;
        dbSupplier.LetterContactContactName = supplier.LetterContactContactName;
        dbSupplier.LetterContactAddress = supplier.LetterContactAddress;
        dbSupplier.LetterContactCity = supplier.LetterContactCity;
        dbSupplier.LetterContactStateId = supplier.LetterContactStateId;
        dbSupplier.LetterContactZipCode = supplier.LetterContactZipCode;
        dbSupplier.LetterContactPhoneNumber = supplier.LetterContactPhoneNumber;
        dbSupplier.LetterContactFaxNumber = supplier.LetterContactFaxNumber;
        dbSupplier.LetterContactEmailAddress = supplier.LetterContactEmailAddress;

        // dbSupplier.UpdatedById = _tenantProvider.UserId;

        await DbContext.SaveChangesAsync();
        return dbSupplier;
    }

    public override async Task<WaterSupplier?> DeleteAsync(int id)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var supplier = await base.DeleteAsync(id);

        if (supplier != null && supplier.DeletedTime != null)
        {
            await DeleteSupplierRecordsAsync(id, supplier.DeletedTime.Value, supplier.DeletedById);
        }

        scope.Complete();

        return supplier;
    }

    public override async Task<WaterSupplier?> ReactivateAsync(int id)
    {
        var supplier = await GetAsync(id, default);

        if (supplier == null || supplier.DeletedTime == null)
        {
            return supplier;
        }

        var deletedTime = supplier.DeletedTime.Value;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await DbContext.WaterSuppliers
            .IgnoreQueryFilters()
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(set => set
                .SetProperty(s => s.DeletedTime, (DateTime?)null)
                .SetProperty(s => s.DeletedById, (int?)null));

        await RestoreSupplierRecordsAsync(id, deletedTime);

        scope.Complete();

        supplier.DeletedTime = null;
        supplier.DeletedById = null;

        return supplier;
    }

    private async Task DeleteSupplierRecordsAsync(int waterSupplierId, DateTime deletedTime, int? deletedById)
    {
        await DbContext.Sites
            .IgnoreQueryFilters()
            .Where(site => site.WaterSupplierId == waterSupplierId && site.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(site => site.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(site => site.DeletedById, deletedById));

        await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(test => test.WaterSupplierId == waterSupplierId && test.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(test => test.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(test => test.DeletedById, deletedById));

        await DbContext.CsiInspections
            .IgnoreQueryFilters()
            .Where(inspection => inspection.WaterSupplierId == waterSupplierId && inspection.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(inspection => inspection.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(inspection => inspection.DeletedById, deletedById));

        await DbContext.FogInspections
            .IgnoreQueryFilters()
            .Where(inspection => inspection.WaterSupplierId == waterSupplierId && inspection.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(inspection => inspection.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(inspection => inspection.DeletedById, deletedById));

        await DbContext.FogTripTickets
            .IgnoreQueryFilters()
            .Where(ticket => ticket.WaterSupplierId == waterSupplierId && ticket.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(ticket => ticket.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(ticket => ticket.DeletedById, deletedById));

        await DbContext.GisAreas
            .IgnoreQueryFilters()
            .Where(area => area.WaterSupplierId == waterSupplierId && area.DeletedTime == null)
            .ExecuteUpdateAsync(set => set
                .SetProperty(area => area.DeletedTime, (DateTime?)deletedTime)
                .SetProperty(area => area.DeletedById, deletedById));
    }

    private async Task RestoreSupplierRecordsAsync(int waterSupplierId, DateTime deletedTime)
    {
        await DbContext.Sites
            .IgnoreQueryFilters()
            .Where(site => site.WaterSupplierId == waterSupplierId && site.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(site => site.DeletedTime, (DateTime?)null)
                .SetProperty(site => site.DeletedById, (int?)null));

        await DbContext.BackflowTests
            .IgnoreQueryFilters()
            .Where(test => test.WaterSupplierId == waterSupplierId && test.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(test => test.DeletedTime, (DateTime?)null)
                .SetProperty(test => test.DeletedById, (int?)null));

        await DbContext.CsiInspections
            .IgnoreQueryFilters()
            .Where(inspection => inspection.WaterSupplierId == waterSupplierId && inspection.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(inspection => inspection.DeletedTime, (DateTime?)null)
                .SetProperty(inspection => inspection.DeletedById, (int?)null));

        await DbContext.FogInspections
            .IgnoreQueryFilters()
            .Where(inspection => inspection.WaterSupplierId == waterSupplierId && inspection.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(inspection => inspection.DeletedTime, (DateTime?)null)
                .SetProperty(inspection => inspection.DeletedById, (int?)null));

        await DbContext.FogTripTickets
            .IgnoreQueryFilters()
            .Where(ticket => ticket.WaterSupplierId == waterSupplierId && ticket.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(ticket => ticket.DeletedTime, (DateTime?)null)
                .SetProperty(ticket => ticket.DeletedById, (int?)null));

        await DbContext.GisAreas
            .IgnoreQueryFilters()
            .Where(area => area.WaterSupplierId == waterSupplierId && area.DeletedTime == deletedTime)
            .ExecuteUpdateAsync(set => set
                .SetProperty(area => area.DeletedTime, (DateTime?)null)
                .SetProperty(area => area.DeletedById, (int?)null));
    }

    public async Task<WaterSupplier?> UpdateOwnAsync(WaterSupplier supplier)
    {
        var dbSupplier = await DbContext.WaterSuppliers
            .SingleOrDefaultAsync(x => x.Id == _tenantProvider.WaterSupplierId);

        if (dbSupplier == null)
        {
            return null;
        }

        dbSupplier.Name = supplier.Name;
        dbSupplier.ContactName = supplier.ContactName;
        dbSupplier.PwsId = supplier.PwsId;
        dbSupplier.Address = supplier.Address;
        dbSupplier.City = supplier.City;
        dbSupplier.StateId = supplier.StateId;
        dbSupplier.ZipCode = supplier.ZipCode;
        dbSupplier.PhoneNumber = supplier.PhoneNumber;
        dbSupplier.FaxNumber = supplier.FaxNumber;
        dbSupplier.EmailAddress = supplier.EmailAddress;
        dbSupplier.UpdatedTime = DateTime.UtcNow;

        dbSupplier.LetterCompanyName = supplier.LetterCompanyName;
        dbSupplier.LetterContactName = supplier.LetterContactName;
        dbSupplier.LetterAddress = supplier.LetterAddress;
        dbSupplier.LetterCity = supplier.LetterCity;
        dbSupplier.LetterStateId = supplier.LetterStateId;
        dbSupplier.LetterZipCode = supplier.LetterZipCode;

        dbSupplier.LetterContactCompanyName = supplier.LetterContactCompanyName;
        dbSupplier.LetterContactContactName = supplier.LetterContactContactName;
        dbSupplier.LetterContactAddress = supplier.LetterContactAddress;
        dbSupplier.LetterContactCity = supplier.LetterContactCity;
        dbSupplier.LetterContactStateId = supplier.LetterContactStateId;
        dbSupplier.LetterContactZipCode = supplier.LetterContactZipCode;
        dbSupplier.LetterContactPhoneNumber = supplier.LetterContactPhoneNumber;
        dbSupplier.LetterContactFaxNumber = supplier.LetterContactFaxNumber;
        dbSupplier.LetterContactEmailAddress = supplier.LetterContactEmailAddress;

        await SaveChangesAsync(logData: true);

        return dbSupplier;
    }

    public async Task<IEnumerable<WaterSupplier>> GetAllMySuppliersAsync(CancellationToken cancellationToken)
    {
        var suppliersQuery = DbContext
            .WaterSupplierUsers
            .IgnoreQueryFilters()
            .Where(supplierUser => supplierUser.UserId == _tenantProvider.UserId)
            .Select(supplierUser => supplierUser.WaterSupplier!);

        var childSupplierQuery = from supplier in DbContext.WaterSuppliers.IgnoreQueryFilters()
                                 join childSupplier in DbContext.WaterSuppliers.IgnoreQueryFilters()
                                 on supplier.Id equals childSupplier.ParentId

                                 join supplierUser in DbContext.WaterSupplierUsers.IgnoreQueryFilters()
                                 on supplier.Id equals supplierUser.WaterSupplierId

                                 where supplierUser.UserId == _tenantProvider.UserId

                                 select childSupplier;

        var grandChildrenQuery = from supplier in DbContext.WaterSuppliers.IgnoreQueryFilters()

                                 join childSupplier in DbContext.WaterSuppliers.IgnoreQueryFilters()
                                 on supplier.Id equals childSupplier.ParentId

                                 join grandChild in DbContext.WaterSuppliers.IgnoreQueryFilters()
                                 on childSupplier.Id equals grandChild.ParentId

                                 join supplierUser in DbContext.WaterSupplierUsers.IgnoreQueryFilters()
                                 on supplier.Id equals supplierUser.WaterSupplierId

                                 where supplierUser.UserId == _tenantProvider.UserId

                                 select grandChild;

        return await suppliersQuery
            .Union(childSupplierQuery)
            .Union(grandChildrenQuery)
            .Where(supplier => supplier.DeletedTime == null)
            .ToListAsync(cancellationToken);
    }
}