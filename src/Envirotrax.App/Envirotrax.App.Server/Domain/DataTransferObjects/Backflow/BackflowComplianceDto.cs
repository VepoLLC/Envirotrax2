using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowComplianceDto : BackflowTestDto
{
    public List<SiteLogDto> Logs { get; set; } = [];

    public int? DaysExpired { get; set; }

    public ComplianceOverdueSeverity ExpiredSeverity { get; set; }
}
