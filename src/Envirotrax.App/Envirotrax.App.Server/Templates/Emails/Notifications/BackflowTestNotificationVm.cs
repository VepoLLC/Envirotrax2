namespace Envirotrax.App.Server.Templates.Emails.Notifications;

public class BackflowTestNotificationVm
{
    public int TestId { get; set; }
    public int? SiteId { get; set; }

    public string Description { get; set; } = null!;
    public string? PropertyAddress { get; set; }
    public string? DeviceDescription { get; set; }
    public string? SerialNumber { get; set; }
    public string? HazardType { get; set; }
    public string? LocationDescription { get; set; }
}
