namespace Envirotrax.App.Server.Templates.Emails.Notifications;

public class BackflowTestNotificationVm
{
    // Distinct tests across every group — a single test can appear in more than one
    // group when it matched several of the recipient's notification settings.
    public int TestCount { get; set; }

    public List<BackflowTestNotificationGroupVm> Groups { get; set; } = [];
}

public class BackflowTestNotificationGroupVm
{
    public string Description { get; set; } = null!;

    public string Color { get; set; } = null!;

    public List<BackflowTestNotificationItemVm> Items { get; set; } = [];
}

public class BackflowTestNotificationItemVm
{
    public int TestId { get; set; }

    public string Title { get; set; } = null!;

    public string? Details { get; set; }

    public string? LocationDescription { get; set; }
}
