
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalSupplierRepository : Repository<ProfessionalWaterSupplier>, IProfessionalSupplierRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public ProfessionalSupplierRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override string GetPrimaryColumnName()
    {
        return nameof(ProfessionalWaterSupplier.WaterSupplierId);
    }

    protected override IQueryable<ProfessionalWaterSupplier> GetListQuery()
    {
        return base.GetListQuery()
            .Include(pws => pws.WaterSupplier)
            .WhereIf(_tenantProvider.ProfessionalId > 0, pws => !pws.IsBanned && !pws.WaterSupplier!.GeneralSettings!.AdministrativeOnly)
            .AsNoTracking();
    }

    protected override IQueryable<ProfessionalWaterSupplier> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(pws => pws.WaterSupplier)
            .WhereIf(_tenantProvider.ProfessionalId > 0, pws => !pws.IsBanned && !pws.WaterSupplier!.GeneralSettings!.AdministrativeOnly)
            .AsNoTracking();
    }

    public override Task<IEnumerable<ProfessionalWaterSupplier>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalWaterSupplier.WaterSupplierId)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalWaterSupplier>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalWaterSupplier, bool>>? filter = null)
    {
        var q = DbContext.ProfessionalWaterSuppliers
            .AsNoTracking()
            .Include(pws => pws.WaterSupplier)
            .Where(pws => pws.ProfessionalId == professionalId);

        if (filter != null)
            q = q.Where(filter);

        var paginated = await q
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AvailableWaterSupplier>> GetAllAvailableSuppliersAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var suppliersQuery = from supplier in DbContext.WaterSuppliers
                             join settings in DbContext.GeneralSettings
                             on supplier.Id equals settings.WaterSupplierId into settingsJoin
                             from settings in settingsJoin.DefaultIfEmpty()
                             where !settings.AdministrativeOnly
                             select new AvailableWaterSupplier
                             {
                                 Id = supplier.Id,
                                 Name = supplier.Name,
                                 StateId = supplier.StateId,
                                 IsActive = supplier.IsActive,

                                 HasBackflowTesting = (bool?)settings.BackflowTesting ?? false,
                                 HasCsiInspection = (bool?)settings.CsiInspections ?? false,
                                 HasWiseGuys = (bool?)settings.WiseGuys ?? false,
                                 HasFogInspection = (bool?)settings.FogProgram ?? false,
                                 HasFogTransportation = (bool?)settings.FogProgram ?? false,

                                 BpatsRequireInsurance = (bool?)settings.BpatsRequireInsurance ?? false,
                                 CsiInspectorsRequireInsurance = (bool?)settings.CsiInspectorsRequireInsurance ?? false,
                                 FogTransportersRequireInsurance = (bool?)settings.FogTransportersRequireInsurance ?? false,
                                 FogVehiclesRequireInspection = (bool?)settings.FogVehiclesRequireInspection ?? false,
                                 FogVehiclesRequirePermit = (bool?)settings.FogVehiclesRequirePermit ?? false,
                                 RequireBackflowTestImages = (bool?)settings.RequireBackflowTestImages ?? false,
                                 RequireCsiInspectionImages = (bool?)settings.RequireCsiInspectionImages ?? false,

                                 BpatsRequireInsuranceAmount = (decimal?)settings.BpatsRequireInsuranceAmount,
                                 CsiInspectorsRequireInsuranceAmount = (decimal?)settings.CsiInspectorsRequireInsuranceAmount,
                                 FogTransportersRequireInsuranceAmount = (decimal?)settings.FogTransportersRequireInsuranceAmount,

                                 BackflowCommercialTestFee = (decimal?)settings.BackflowCommercialTestFee,
                                 BackflowResidentialTestFee = (decimal?)settings.BackflowResidentialTestFee,
                                 CsiCommercialInspectionFee = (decimal?)settings.CsiCommercialInspectionFee,
                                 CsiResidentialInspectionFee = (decimal?)settings.CsiResidentialInspectionFee,
                                 FogTransportFee = (decimal?)settings.FogTransportFee,
                                 FogInspectorFee = (decimal?)settings.FogInspectorFee
                             };

        var paginated = await suppliersQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}