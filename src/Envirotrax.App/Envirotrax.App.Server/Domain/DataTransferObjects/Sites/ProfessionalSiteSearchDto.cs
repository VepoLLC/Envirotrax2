namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class ProfessionalSiteSearchDto
{
    public bool WorkedOnly { get; set; }
    public bool ScheduledOnly { get; set; }
    public DateTime? ScheduledFrom { get; set; }
    public DateTime? ScheduledTo { get; set; }
}
