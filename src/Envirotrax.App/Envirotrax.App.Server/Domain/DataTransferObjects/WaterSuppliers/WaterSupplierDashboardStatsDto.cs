namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierDashboardStatsDto
{
    // Accounts
    public int WiseGuyCount { get; set; }
    public int CsiInspectorCount { get; set; }
    public int BpatCount { get; set; }
    public int FogTransporterCount { get; set; }
    public int FogInspectorCount { get; set; }

    // Licensing
    public int UnverifiedLicenseCount { get; set; }
    public int ExpiredLicenseCount { get; set; }
    public int ExpiringLicenseCount { get; set; }

    // Required Validations
    public int InsurancePolicyCount { get; set; }
    public int TestGaugeCount { get; set; }
    public int TransporterRegistrationCount { get; set; }
}
