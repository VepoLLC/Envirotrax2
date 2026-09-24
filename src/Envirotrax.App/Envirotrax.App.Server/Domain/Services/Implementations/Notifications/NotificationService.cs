using AutoMapper;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class NotificationService : Service<Notification, NotificationDto>, INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(IMapper mapper, INotificationRepository repository)
        : base(mapper, repository)
    {
        _repository = repository;
    }

    public Task<List<Notification>> AddAsync(List<Notification> notifications, CancellationToken cancellationToken)
    {
        return _repository.AddRangeAsync(notifications);
    }

    public Task MarkSentAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        return _repository.MarkSentAsync(ids, cancellationToken);
    }
}
