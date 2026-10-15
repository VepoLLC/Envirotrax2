namespace Envirotrax.App.Server.Templates.Emails.Notifications;

public class NotificationDigestVm
{
    public string Period { get; set; } = null!;

    public string DateGenerated { get; set; } = null!;

    public string WaterSupplierName { get; set; } = null!;

    public string Account { get; set; } = null!;

    public string BackflowTestType { get; set; } = null!;

    public string PropertyType { get; set; } = null!;

    public string NotificationFilter { get; set; } = null!;

    public string HazardTypes { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Color { get; set; } = null!;

    public List<BackflowTestNotificationItemVm> Items { get; set; } = [];
}
