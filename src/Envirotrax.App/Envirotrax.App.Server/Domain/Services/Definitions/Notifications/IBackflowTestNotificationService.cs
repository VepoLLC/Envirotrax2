namespace Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

public interface IBackflowTestNotificationService
{
    Task StartCheckingNotificationsAsync(IEnumerable<int> testIds, CancellationToken cancellationToken);
}
