using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class GeneralSettings : TenantModel<WaterSupplier>
{
    // Program Settings - Checkboxes
    public bool PrivacyRequired { get; set; }
    public bool NewSitesLocked { get; set; }
    public bool AdministrativeOnly { get; set; }
    public bool WiseGuys { get; set; }
    public bool BackflowTesting { get; set; }
    public bool CsiInspections { get; set; }
    public bool FogProgram { get; set; }

    // Insurance and License Requirements
    public bool BpatsRequireInsurance { get; set; }
    public bool BpatsRequireIrrigationLicense { get; set; }
    public bool CsiInspectorsRequireInsurance { get; set; }
    public bool FogTransportersRequireInsurance { get; set; }
    public bool FogVehiclesRequirePermit { get; set; }
    public bool FogVehiclesRequireInspection { get; set; }

    // Lock Registration Options
    public bool LockBpatRegistrations { get; set; }
    public bool LockCsiRegistrations { get; set; }
    public bool LockFogInspectorRegistrations { get; set; }
    public bool LockFogTransporterRegistrations { get; set; }

    // Insurance Amount Fields
    [Precision(19, 4)]
    public decimal BpatsRequireInsuranceAmount { get; set; }

    [Precision(19, 4)]
    public decimal CsiInspectorsRequireInsuranceAmount { get; set; }

    [Precision(19, 4)]
    public decimal FogTransportersRequireInsuranceAmount { get; set; }

    // Submission Fees - Total Fee and WS Share
    [Precision(19, 4)]
    public decimal BackflowCommercialTestFee { get; set; }

    [Precision(19, 4)]
    public decimal BackflowCommercialTestFeeWsShare { get; set; }

    [Precision(19, 4)]
    public decimal BackflowResidentialTestFee { get; set; }

    [Precision(19, 4)]
    public decimal BackflowResidentialTestFeeWsShare { get; set; }

    [Precision(19, 4)]
    public decimal CsiCommercialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal CsiCommercialInspectionFeeWsShare { get; set; }

    [Precision(19, 4)]
    public decimal CsiResidentialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal CsiResidentialInspectionFeeWsShare { get; set; }

    [Precision(19, 4)]
    public decimal FogTransportFee { get; set; }

    [Precision(19, 4)]
    public decimal FogTransportFeeWsShare { get; set; }
}
