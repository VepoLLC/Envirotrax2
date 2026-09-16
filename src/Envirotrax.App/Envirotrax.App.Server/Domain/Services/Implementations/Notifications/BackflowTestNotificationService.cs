using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Templates.Emails.Notifications;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Logging;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class BackflowTestNotificationService : IBackflowTestNotificationService
{
    private readonly IBackflowTestRepository _testRepository;
    private readonly INotificationSettingService _notificationSettingService;
    private readonly IBackflowTestNotificationMatcher _matcher;
    private readonly INotificationService _notificationService;
    private readonly IEmailService _emailService;
    private readonly ILogger<BackflowTestNotificationService> _logger;

    public BackflowTestNotificationService(
        IBackflowTestRepository testRepository,
        INotificationSettingService notificationSettingService,
        IBackflowTestNotificationMatcher matcher,
        INotificationService notificationService,
        IEmailService emailService,
        ILogger<BackflowTestNotificationService> logger)
    {
        _testRepository = testRepository;
        _notificationSettingService = notificationSettingService;
        _matcher = matcher;
        _notificationService = notificationService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task StartCheckingNotificationsAsync(IEnumerable<int> testIds, CancellationToken cancellationToken)
    {
        var tests = await _testRepository.GetByIdsAsync(testIds, cancellationToken);

        var settingsByWaterSupplierId = new Dictionary<int, List<NotificationSetting>>();

        foreach (var test in tests)
        {
            var ownSettings = await GetSettingsAsync(test.WaterSupplierId, settingsByWaterSupplierId, cancellationToken);

            var parentWaterSupplierId = test.WaterSupplier?.Parent?.Id;
            var parentSettings = parentWaterSupplierId.HasValue
                ? await GetSettingsAsync(parentWaterSupplierId.Value, settingsByWaterSupplierId, cancellationToken)
                : [];

            if (ownSettings.Count == 0 && parentSettings.Count == 0)
            {
                continue;
            }

            var previousTest = await _testRepository.FindPreviousTestAsync(test, cancellationToken);

            // Own water supplier's settings first, then (if any) the parent's — a parent's setting
            // fires the same way a regular one would, just attributed to the parent's WaterSupplierId.
            await ProcessSettingsAsync(test, test.WaterSupplierId, ownSettings, previousTest, cancellationToken);

            if (parentWaterSupplierId.HasValue)
            {
                await ProcessSettingsAsync(test, parentWaterSupplierId.Value, parentSettings, previousTest, cancellationToken);
            }
        }
    }

    private async Task<List<NotificationSetting>> GetSettingsAsync(
        int waterSupplierId, Dictionary<int, List<NotificationSetting>> cache, CancellationToken cancellationToken)
    {
        if (!cache.TryGetValue(waterSupplierId, out var settings))
        {
            settings = await _notificationSettingService.GetCandidateSettingsAsync(waterSupplierId, cancellationToken);
            cache[waterSupplierId] = settings;
        }

        return settings;
    }

    private async Task ProcessSettingsAsync(
        BackflowTest test,
        int waterSupplierId,
        List<NotificationSetting> settings,
        BackflowTest? previousTest,
        CancellationToken cancellationToken)
    {
        foreach (var setting in settings)
        {
            var notification = _matcher.BuildMatch(setting, test, previousTest);

            if (notification == null)
            {
                continue;
            }

            ApplyRecordMetadata(notification, test, waterSupplierId);

            await _notificationService.AddAsync(notification, cancellationToken);

            if (notification.Interval != NotificationInterval.Immediate)
            {
                continue;
            }

            await TrySendImmediateEmailAsync(notification, setting, test, cancellationToken);
        }
    }

    private static void ApplyRecordMetadata(Notification notification, BackflowTest test, int waterSupplierId)
    {
        notification.WaterSupplierId = waterSupplierId;
        notification.ModuleType = NotificationModuleType.Backflow;
        notification.RecordId = test.Id;
        notification.ParentWaterSupplierId = test.WaterSupplier?.Parent?.Id;
        notification.PropertyDescription = BuildPropertyDescription(test);
        notification.RecordDescription = BuildDeviceDescription(test);
    }

    private static string? BuildPropertyDescription(BackflowTest test)
    {
        var parts = new[] { test.PropertyStreetNumber, test.PropertyStreetName, test.PropertyCity }
            .Where(part => !string.IsNullOrWhiteSpace(part));

        var address = string.Join(" ", parts);

        return string.IsNullOrEmpty(address) ? null : address;
    }

    private static string? BuildDeviceDescription(BackflowTest test)
    {
        var parts = new[] { test.Manufacturer, test.Model, test.Size, test.DeviceType }
            .Where(part => !string.IsNullOrWhiteSpace(part));

        var description = string.Join(" ", parts);

        return string.IsNullOrEmpty(description) ? null : description;
    }

    private async Task TrySendImmediateEmailAsync(
        Notification notification, NotificationSetting setting, BackflowTest test, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(setting.User?.EmailAddress))
        {
            return;
        }

        try
        {
            var vm = new BackflowTestNotificationVm
            {
                TestId = test.Id,
                SiteId = test.SiteId,
                Description = notification.Description,
                PropertyAddress = notification.PropertyDescription,
                DeviceDescription = notification.RecordDescription,
                SerialNumber = test.SerialNumber,
                HazardType = test.HazardType,
                LocationDescription = test.LocationDescription
            };

            await _emailService.SendAsync(new EmailDto<BackflowTestNotificationVm>
            {
                Recipients = [setting.User.EmailAddress],
                Subject = $"Envirotrax Notification - {notification.Description}",
                TemplateId = "Notifications.BackflowTestNotification",
                TemplateData = vm
            });

            await _notificationService.MarkSentAsync(notification.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification email for Notification {NotificationId}.", notification.Id);
        }
    }
}
