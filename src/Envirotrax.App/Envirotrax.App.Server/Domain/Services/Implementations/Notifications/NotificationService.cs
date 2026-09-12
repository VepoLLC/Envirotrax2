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

    public Task<Notification> AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        return _repository.AddAsync(notification);
    }

    public Task MarkSentAsync(int id, CancellationToken cancellationToken)
    {
        return _repository.MarkSentAsync(id, cancellationToken);
    }
}
