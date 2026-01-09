
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
            .Include(supplier => supplier.LetterAddress)
            .Include(supplier => supplier.LetterContact)
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
            .Include(x => x.LetterAddress)
            .Include(x => x.LetterContact)
            .SingleOrDefaultAsync(x =>
                x.ParentId == _tenantProvider.WaterSupplierId &&
                x.Id == supplier.Id);

        if (dbSupplier == null)
            return null;

        UpdateSupplier(dbSupplier, supplier);
        UpdateLetterAddress(dbSupplier, supplier.LetterAddress);
        UpdateLetterContact(dbSupplier, supplier.LetterContact);

        await DbContext.SaveChangesAsync();
        return dbSupplier;
    }

    private void UpdateSupplier(WaterSupplier dbSupplier, WaterSupplier supplier)
    {
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
        // dbSupplier.UpdatedById = _tenantProvider.UserId;
    }

    private void UpdateLetterAddress(WaterSupplier dbSupplier, LetterAddress? source)
    {
        if (source == null)
            return;

        if (dbSupplier.LetterAddress == null)
        {
            dbSupplier.LetterAddress = new LetterAddress();
        }

        dbSupplier.LetterAddress.CompanyName = source.CompanyName;
        dbSupplier.LetterAddress.ContactName = source.ContactName;
        dbSupplier.LetterAddress.Address = source.Address;
        dbSupplier.LetterAddress.City = source.City;
        dbSupplier.LetterAddress.StateId = source.StateId;
        dbSupplier.LetterAddress.ZipCode = source.ZipCode;
    }

    private void UpdateLetterContact(WaterSupplier dbSupplier, LetterContact? source)
    {
        if (source == null)
            return;

        if (dbSupplier.LetterContact == null)
        {
            dbSupplier.LetterContact = new LetterContact();
        }

        dbSupplier.LetterContact.CompanyName = source.CompanyName;
        dbSupplier.LetterContact.ContactName = source.ContactName;
        dbSupplier.LetterContact.Address = source.Address;
        dbSupplier.LetterContact.City = source.City;
        dbSupplier.LetterContact.StateId = source.StateId;
        dbSupplier.LetterContact.ZipCode = source.ZipCode;
        dbSupplier.LetterContact.PhoneNumber = source.PhoneNumber;
        dbSupplier.LetterContact.FaxNumber = source.FaxNumber;
        dbSupplier.LetterContact.EmailAddress = source.EmailAddress;
    }
}