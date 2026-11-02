using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class NotificationService : Service<Notification, NotificationDto>, INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly IBackflowTestRepository _testRepository;

    public NotificationService(IMapper mapper, INotificationRepository repository, IBackflowTestRepository testRepository)
        : base(mapper, repository)
    {
        _repository = repository;
        _testRepository = testRepository;
    }

    public override async Task<IPagedData<NotificationDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<Notification, NotificationDto>(Mapper);
        query.Sort = query.ConvertSortProperties<Notification, NotificationDto>(Mapper);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(Notification.Description)] = SortOperator.Asc;
            query.Sort[nameof(Notification.CreatedTime)] = SortOperator.Desc;
        }

        var notifications = await _repository.GetAllAsync(pageInfo, query, cancellationToken);
        var dtos = notifications.Select(notification => MapToDto(notification)!).ToList();

        await AttachTestsAsync(dtos, cancellationToken);

        return dtos.ToPagedData(pageInfo);
    }

    private async Task AttachTestsAsync(List<NotificationDto> notifications, CancellationToken cancellationToken)
    {
        var backflowNotifications = notifications
            .Where(notification => notification.ModuleType == NotificationModuleType.Backflow)
            .ToList();

        if (backflowNotifications.Count == 0)
        {
            return;
        }

        var testIds = backflowNotifications.Select(notification => notification.RecordId).Distinct();
        var tests = await _testRepository.GetForNotificationListAsync(testIds, cancellationToken);
        var testsById = tests.ToDictionary(test => test.Id);

        foreach (var notification in backflowNotifications)
        {
            if (testsById.TryGetValue(notification.RecordId, out var test))
            {
                notification.Test = Mapper.Map<NotificationTestDto>(test);
            }
        }
    }

    public Task<List<Notification>> AddAsync(List<Notification> notifications, CancellationToken cancellationToken)
    {
        return _repository.AddRangeAsync(notifications);
    }

    public Task MarkSentAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        return _repository.MarkSentAsync(ids, cancellationToken);
    }

    public async Task<NotificationDto?> SetHiddenAsync(int id, bool hidden, CancellationToken cancellationToken)
    {
        var notification = await _repository.SetHiddenAsync(id, hidden, cancellationToken);

        return MapToDto(notification);
    }
}
