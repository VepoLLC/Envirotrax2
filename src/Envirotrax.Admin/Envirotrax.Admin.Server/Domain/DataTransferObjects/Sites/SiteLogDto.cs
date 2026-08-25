namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

public class SiteLogDto
{
    public int Id { get; set; }

    public int LogType { get; set; }

    public string? NoteText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public int? AssemblyId { get; set; }

    public string? FileAttachmentName { get; set; }

    public DateTime CreatedTime { get; set; }

    public SiteLogUserDto? CreatedBy { get; set; }
}

public class SiteLogUserDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}
