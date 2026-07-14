using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class PropertyLogDto : IDto
{
    public int Id { get; set; }

    public SiteDto? Site { get; set; }

    public SiteLogType LogType { get; set; }
    public string? NoteText { get; set; }
    public DateTime? ReviewDate { get; set; }
    public SiteLogReviewDateStatus ReviewDateStatus { get; set; }

    public int? AssemblyId { get; set; }
    public ReferencedBackflowTestDto? Assembly { get; set; }

    public string? FileAttachmentName { get; set; }
    public string? FileAttachmentPath { get; set; }
    public string? Url { get; set; }
    public bool SkipFile { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedTime { get; set; }
    public AppUserDto? CreatedBy { get; set; }
}
