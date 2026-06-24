using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class SiteLogDto : IDto
{
    public int Id { get; set; }
    public ReferencedSiteDto Site { get; set; } = null!;
    public SiteLogType LogType { get; set; }
    public string? NoteText { get; set; }
    public DateTime? ReviewDate { get; set; }
    public int? AssemblyId { get; set; }
    public ReferencedBackflowTestDto? Assembly { get; set; }
    public string? FileAttachmentName { get; set; }
    public string? FileAttachmentPath { get; set; }
    public string? Url { get; set; }
    public bool SkipFile { get; set; }
    public SiteLogReviewDateStatus ReviewDateStatus { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
    public AppUserDto? CreatedBy { get; set; }
}

public class ReferencedBackflowTestDto
{
    public int? Id { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Size { get; set; }
    public string? DeviceType { get; set; }
}
