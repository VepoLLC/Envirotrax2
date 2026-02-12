
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals;
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

        dbRecord.CommercialFee = profesisonalSupplier.CommercialFee;
        dbRecord.ResidentialFee = profesisonalSupplier.ResidentialFee;
        dbRecord.FogFee = profesisonalSupplier.FogFee;
        dbRecord.HasBackflowTesting = profesisonalSupplier.HasBackflowTesting;
        dbRecord.HasCsiInpection = profesisonalSupplier.HasCsiInpection;
        dbRecord.HasFogInspection = profesisonalSupplier.HasFogInspection;
        dbRecord.HasFogTransportation = profesisonalSupplier.HasFogTransportation;

        return dbRecord;
    }
}