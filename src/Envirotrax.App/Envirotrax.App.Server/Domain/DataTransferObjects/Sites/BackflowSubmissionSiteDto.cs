using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class BackflowSubmissionSiteDto : SiteDto
{
    public bool IncludeWsAccountNumbers { get; set; }

    public List<BackflowTestDto> Assemblies { get; set; } = [];
}
