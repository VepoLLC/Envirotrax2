
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class GeneralSettingsRepository : Repository<GeneralSettings>, IGeneralSettingsRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public GeneralSettingsRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<GeneralSettings> GetListQuery()
    {
        return base.GetListQuery()
            .Where(settings => settings.WaterSupplierId == _tenantProvider.WaterSupplierId);
    }

    protected override IQueryable<GeneralSettings> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Where(settings => settings.WaterSupplierId == _tenantProvider.WaterSupplierId);
    }

    public override Task<GeneralSettings> AddAsync(GeneralSettings settings)
    {
        settings.WaterSupplierId = _tenantProvider.WaterSupplierId;
        return base.AddAsync(settings);
    }

    public override async Task<GeneralSettings?> UpdateAsync(GeneralSettings settings)
    {
        var dbSettings = await DbContext.GeneralSettings
            .SingleOrDefaultAsync(s => s.WaterSupplierId == _tenantProvider.WaterSupplierId);

        if (dbSettings != null)
        {
            dbSettings.PrivacyRequired = settings.PrivacyRequired;
            dbSettings.NewSitesLocked = settings.NewSitesLocked;
            dbSettings.AdministrativeOnly = settings.AdministrativeOnly;
            dbSettings.WiseGuys = settings.WiseGuys;
            dbSettings.BackflowTesting = settings.BackflowTesting;
            dbSettings.CsiInspections = settings.CsiInspections;
            dbSettings.FogProgram = settings.FogProgram;

            dbSettings.BpatsRequireInsurance = settings.BpatsRequireInsurance;
            dbSettings.BpatsRequireIrrigationLicense = settings.BpatsRequireIrrigationLicense;
            dbSettings.CsiInspectorsRequireInsurance = settings.CsiInspectorsRequireInsurance;
            dbSettings.FogTransportersRequireInsurance = settings.FogTransportersRequireInsurance;
            dbSettings.FogVehiclesRequirePermit = settings.FogVehiclesRequirePermit;
            dbSettings.FogVehiclesRequireInspection = settings.FogVehiclesRequireInspection;

            dbSettings.LockBpatRegistrations = settings.LockBpatRegistrations;
            dbSettings.LockCsiRegistrations = settings.LockCsiRegistrations;
            dbSettings.LockFogInspectorRegistrations = settings.LockFogInspectorRegistrations;
            dbSettings.LockFogTransporterRegistrations = settings.LockFogTransporterRegistrations;

            dbSettings.BpatsRequireInsuranceAmount = settings.BpatsRequireInsuranceAmount;
            dbSettings.CsiInspectorsRequireInsuranceAmount = settings.CsiInspectorsRequireInsuranceAmount;
            dbSettings.FogTransportersRequireInsuranceAmount = settings.FogTransportersRequireInsuranceAmount;

            dbSettings.BackflowCommercialTestFee = settings.BackflowCommercialTestFee;
            dbSettings.BackflowCommercialTestFeeWsShare = settings.BackflowCommercialTestFeeWsShare;
            dbSettings.BackflowResidentialTestFee = settings.BackflowResidentialTestFee;
            dbSettings.BackflowResidentialTestFeeWsShare = settings.BackflowResidentialTestFeeWsShare;
            dbSettings.CsiCommercialInspectionFee = settings.CsiCommercialInspectionFee;
            dbSettings.CsiCommercialInspectionFeeWsShare = settings.CsiCommercialInspectionFeeWsShare;
            dbSettings.CsiResidentialInspectionFee = settings.CsiResidentialInspectionFee;
            dbSettings.CsiResidentialInspectionFeeWsShare = settings.CsiResidentialInspectionFeeWsShare;
            dbSettings.FogTransportFee = settings.FogTransportFee;
            dbSettings.FogTransportFeeWsShare = settings.FogTransportFeeWsShare;

            await DbContext.SaveChangesAsync();
        }

        return dbSettings;
    }
}
