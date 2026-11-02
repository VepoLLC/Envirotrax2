using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

public class NotificationTestDto
{
    public int Id { get; set; }
    public int? SiteId { get; set; }

    public string? PropertyBusinessName { get; set; }
    public int PropertyType { get; set; }
    public string? PropertyStreetNumber { get; set; }
    public string? PropertyStreetName { get; set; }
    public string? PropertyNumber { get; set; }
    public string? PropertyCity { get; set; }
    public ReferencedStateDto? PropertyState { get; set; }
    public string? PropertyZip { get; set; }

    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Size { get; set; }
    public string? DeviceType { get; set; }
    public string? SerialNumber { get; set; }

    public DateTime? TestDate { get; set; }
    public bool RenewalRequired { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool OutOfService { get; set; }
}
