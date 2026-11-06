namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class LicenseAdminCountsDto : LicenseCountsDto
{
    public int InsuranceCount { get; set; }
    public int GaugeCount { get; set; }
}
