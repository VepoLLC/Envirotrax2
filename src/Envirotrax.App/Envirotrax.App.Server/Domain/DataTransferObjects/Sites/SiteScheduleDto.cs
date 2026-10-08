using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class SiteScheduleDto : IDto
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public ProfessionalType ProfessionalType { get; set; }
    public DateTime ScheduleDate { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
}
