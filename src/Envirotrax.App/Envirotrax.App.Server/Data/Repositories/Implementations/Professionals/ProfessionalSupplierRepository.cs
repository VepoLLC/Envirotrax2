
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalSupplierRepository : Repository<ProfessionalWaterSupplier>, IProfessionalSupplierRepository
{

    public ProfessionalSupplierRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override string GetPrimaryColumnName()
    {
        return nameof(ProfessionalWaterSupplier.WaterSupplierId);
    }

    public async Task<IEnumerable<AvailableWaterSupplier>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var suppliersQuery = from supplier in DbContext.WaterSuppliers
                             join settings in DbContext.GeneralSettings
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

                                 BpatsRequireInsurance = (bool?)settings.BpatsRequireInsurance ?? false,
                                 CsiInspectorsRequireInsurance = (bool?)settings.CsiInspectorsRequireInsurance ?? false,
                                 FogTransportersRequireInsurance = (bool?)settings.FogTransportersRequireInsurance ?? false,
                                 FogVehiclesRequireInspection = (bool?)settings.FogVehiclesRequireInspection ?? false,
                                 FogVehiclesRequirePermit = (bool?)settings.FogVehiclesRequirePermit ?? false,

                                 BpatsRequireInsuranceAmount = (decimal?)settings.BpatsRequireInsuranceAmount,
                                 CsiInspectorsRequireInsuranceAmount = (decimal?)settings.CsiInspectorsRequireInsuranceAmount,
                                 FogTransportersRequireInsuranceAmount = (decimal?)settings.FogTransportersRequireInsuranceAmount,

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
}