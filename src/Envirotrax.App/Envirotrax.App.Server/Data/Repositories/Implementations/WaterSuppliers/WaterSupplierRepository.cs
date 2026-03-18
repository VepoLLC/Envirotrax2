
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

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
            .Where(supplier => supplier.ParentId == _tenantProvider.WaterSupplierId)
            .AsNoTracking();
    }

    protected override IQueryable<WaterSupplier> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(supplier => supplier.Parent)
            .Where(supplier => supplier.ParentId == _tenantProvider.WaterSupplierId);
    }

    public override Task<WaterSupplier> AddAsync(WaterSupplier supplier)
    {
        supplier.ParentId = _tenantProvider.WaterSupplierId;
        return base.AddAsync(supplier);
    }

    public override async Task<WaterSupplier?> UpdateAsync(WaterSupplier supplier)
    {
        var dbSupplier = await DbContext.WaterSuppliers
            .SingleOrDefaultAsync(x =>
                x.ParentId == _tenantProvider.WaterSupplierId &&
                x.Id == supplier.Id);

        if (dbSupplier == null)
            return null;

        dbSupplier.ParentId = _tenantProvider.WaterSupplierId;
        dbSupplier.Name = supplier.Name;
        dbSupplier.Domain = supplier.Domain;
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
        dbSupplier.LetterContactName = supplier.LetterContactContactName;
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

    private IQueryable<WaterSupplier> GetMySuppliersQuery()
    {
        if (_tenantProvider.ProfessionalId > 0)
        {
            return DbContext
                .ProfessionalWaterSuppliers
                .IgnoreQueryFilters()
                .Where(professionalSupplier => professionalSupplier.ProfessionalId == _tenantProvider.ProfessionalId)
                .Select(professionalSupplier => professionalSupplier.WaterSupplier!);
        }

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

        return suppliersQuery.Union(childSupplierQuery);
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
            .ToListAsync(cancellationToken);
    }
}