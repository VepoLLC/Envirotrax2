namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class ProfessionalSiteSearchDto
{
    public bool WorkedOnly { get; set; }
    public bool ScheduledOnly { get; set; }
    public DateTime? ScheduledFrom { get; set; }
    public DateTime? ScheduledTo { get; set; }
    public DateTime? CsiRenewalFrom { get; set; }
    public DateTime? CsiRenewalTo { get; set; }
    public DateTime? FogInspectionExpirationFrom { get; set; }
    public DateTime? FogInspectionExpirationTo { get; set; }
}
