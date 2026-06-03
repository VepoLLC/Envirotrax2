namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public class LicenseCountsDto
{
    public int UnverifiedCount { get; set; }
    public int ExpiredCount { get; set; }
    public int ExpiringCount { get; set; }
}
