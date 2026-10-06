namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class ProfessionalSiteDto : SiteDto
{
    public SiteScheduleDto? Schedule { get; set; }
}
