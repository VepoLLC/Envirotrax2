namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class ProfessionalSiteDto : SiteDto
{
    public IEnumerable<SiteScheduleDto> Schedules { get; set; } = [];
}
