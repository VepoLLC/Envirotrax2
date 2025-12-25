
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
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
        var dbSupplier = await DbContext.WaterSuppliers.SingleOrDefaultAsync(t => t.ParentId == _tenantProvider.WaterSupplierId && t.Id == supplier.Id);

        if (dbSupplier != null)
        {
            dbSupplier.ParentId = _tenantProvider.WaterSupplierId;
            dbSupplier.Name = supplier.Name;
            dbSupplier.Domain = supplier.Domain;

            await DbContext.SaveChangesAsync();
        }

        return dbSupplier;
    }

    private IQueryable<WaterSupplier> GetMySuppliersQuery()
    {
        if (_tenantProvider.ContractorId > 0)
        {
            return DbContext
                .WaterSupplierContractors
                .IgnoreQueryFilters()
                .Where(contractorSupplier => contractorSupplier.ContractorId == _tenantProvider.ContractorId)
                .Select(contractorSupplier => contractorSupplier.WaterSupplier!);
        }

        return DbContext
            .WaterSupplierUsers
            .IgnoreQueryFilters()
            .Where(supplierUser => supplierUser.UserId == _tenantProvider.UserId)
            .Select(supplierUser => supplierUser.WaterSupplier!);
    }

    public async Task<IEnumerable<WaterSupplier>> GetAllMySuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await GetMySuppliersQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}