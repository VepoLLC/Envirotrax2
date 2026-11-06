namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class LicenseAdminCountsDto
{
    public int UnverifiedCount { get; set; }

    public int ExpiredCount { get; set; }

    public int ExpiringCount { get; set; }

    public int InsuranceCount { get; set; }

    public int GaugeCount { get; set; }
}
