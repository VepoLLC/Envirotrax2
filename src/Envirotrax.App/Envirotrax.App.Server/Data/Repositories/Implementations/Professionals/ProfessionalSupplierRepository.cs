
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalSupplierRepository : IProfessionalSupplierRepository
{
    private readonly TenantDbContext _dbContext;

    public ProfessionalSupplierRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<IEnumerable<AvailableWaterSupplier>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var suppliersQuery = from supplier in _dbContext.WaterSuppliers
                             join settings in _dbContext.GeneralSettings
                             on supplier.Id equals settings.WaterSupplierId into settingsJoin
                             from settings in settingsJoin.DefaultIfEmpty()
                             select new AvailableWaterSupplier
                             {
                                 Id = supplier.Id,
                                 Name = supplier.Name,
                                 StateId = supplier.StateId,
                                 HasBackflowTesting = (bool?)settings.BackflowTesting ?? false,
                                 HasCsiInpection = (bool?)settings.CsiInspections ?? false,
                                 HasWiseGuys = (bool?)settings.WiseGuys ?? false,
                                 HasFogInspection = (bool?)settings.FogProgram ?? false,
                                 HasFogTransportation = (bool?)settings.FogProgram ?? false,
                                 BackflowCommercialTestFee = (decimal?)settings.BackflowCommercialTestFee,
                                 BackflowResidentialTestFee = (decimal?)settings.BackflowResidentialTestFee,
                                 CsiCommercialInspectionFee = (decimal?)settings.CsiCommercialInspectionFee,
                                 CsiResidentialInspectionFee = (decimal?)settings.CsiResidentialInspectionFee,
                                 FogTransportFee = (decimal?)settings.FogTransportFee
                             };

        var paginated = await suppliersQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalWaterSupplier>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ProfessionalWaterSuppliers.ToListAsync(cancellationToken);
    }

    public async Task<ProfessionalWaterSupplier> AddOrUpdateAsync(ProfessionalWaterSupplier profesisonalSupplier)
    {
        var dbRecord = await _dbContext
            .ProfessionalWaterSuppliers
            .SingleOrDefaultAsync(x => x.WaterSupplierId == profesisonalSupplier.WaterSupplierId);

        if (dbRecord == null)
        {
            dbRecord = new()
            {
                WaterSupplierId = profesisonalSupplier.WaterSupplierId
            };
        }

        dbRecord.BackflowCommercialTestFee = profesisonalSupplier.BackflowCommercialTestFee;
        dbRecord.BackflowResidentialTestFee = profesisonalSupplier.BackflowResidentialTestFee;
        dbRecord.FogTransportFee = profesisonalSupplier.FogTransportFee;
        dbRecord.CsiCommercialInspectionFee = profesisonalSupplier.CsiCommercialInspectionFee;
        dbRecord.CsiResidentialInspectionFee = profesisonalSupplier.CsiResidentialInspectionFee;

        dbRecord.HasBackflowTesting = profesisonalSupplier.HasBackflowTesting;
        dbRecord.HasCsiInpection = profesisonalSupplier.HasCsiInpection;
        dbRecord.HasFogInspection = profesisonalSupplier.HasFogInspection;
        dbRecord.HasFogTransportation = profesisonalSupplier.HasFogTransportation;

        return dbRecord;
    }
}