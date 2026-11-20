namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

public class SiteLogDto
{
    public int Id { get; set; }

    public int LogType { get; set; }

    public string? NoteText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public SiteLogAssemblyDto? Assembly { get; set; }

    public bool SkipFile { get; set; }

    public DateTime CreatedTime { get; set; }
}

public class SiteLogAssemblyDto
{
    public int? Id { get; set; }

    public string? SerialNumber { get; set; }

    public string? Manufacturer { get; set; }

    public string? Model { get; set; }

    public string? Size { get; set; }

    public string? DeviceType { get; set; }
}
