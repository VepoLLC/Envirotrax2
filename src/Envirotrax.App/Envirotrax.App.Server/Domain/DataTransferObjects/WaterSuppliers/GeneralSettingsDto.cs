
namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers
{
    public class GeneralSettingsDto : IDto
    {
        public int Id { get; set; }
        public int WaterSupplierId { get; set; }

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
        public decimal BpatsRequireInsuranceAmount { get; set; }
        public decimal CsiInspectorsRequireInsuranceAmount { get; set; }
        public decimal FogTransportersRequireInsuranceAmount { get; set; }

        // Submission Fees - Total Fee and WS Share
        public decimal BackflowCommercialTestFee { get; set; }
        public decimal BackflowCommercialTestFeeWsShare { get; set; }
        public decimal BackflowResidentialTestFee { get; set; }
        public decimal BackflowResidentialTestFeeWsShare { get; set; }
        public decimal CsiCommercialInspectionFee { get; set; }
        public decimal CsiCommercialInspectionFeeWsShare { get; set; }
        public decimal CsiResidentialInspectionFee { get; set; }
        public decimal CsiResidentialInspectionFeeWsShare { get; set; }
        public decimal FogTransportFee { get; set; }
        public decimal FogTransportFeeWsShare { get; set; }
    }
}
