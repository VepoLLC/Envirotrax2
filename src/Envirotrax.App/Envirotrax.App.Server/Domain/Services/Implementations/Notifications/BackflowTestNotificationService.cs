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

    // Matching and delivery are kept apart on purpose: every test is matched and recorded first, and only
    // then is a single email sent per recipient. Sending inside the matching loop produced one email per
    // (test x setting) pair, so a checkout of 17 tests could send dozens of emails to the same person.
    //
    // Notifications are a side effect of whatever triggers them (checkout, for instance), so failures
    // here are logged and swallowed — a notification problem must never fail the operation that caused it.
    public async Task StartCheckingNotificationsAsync(IEnumerable<int> testIds, CancellationToken cancellationToken)
    {
        List<BackflowTest> tests;

        try
        {
            tests = await _testRepository.GetByIdsAsync(testIds, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load backflow tests for notification checking.");
            return;
        }

        var matches = await MatchAllAsync(tests, cancellationToken);

        await SendImmediateEmailsAsync(matches, cancellationToken);
    }

    private async Task<List<NotificationMatch>> MatchAllAsync(List<BackflowTest> tests, CancellationToken cancellationToken)
    {
        var settingsByWaterSupplierId = await GetSettingsAsync(tests, cancellationToken);

        if (settingsByWaterSupplierId.Count == 0)
        {
            return [];
        }

        var previousTests = await _testRepository.FindPreviousTestsAsync(tests, cancellationToken);
        var matches = new List<NotificationMatch>();

        foreach (var test in tests)
        {
            try
            {
                previousTests.TryGetValue(test.Id, out var previousTest);

                matches.AddRange(MatchTest(test, previousTest, settingsByWaterSupplierId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process notifications for backflow test {TestId}.", test.Id);
            }
        }

        if (matches.Count > 0)
        {
            await _notificationService.AddAsync([.. matches.Select(match => match.Notification)], cancellationToken);
        }

        return matches;
    }

    // Every water supplier the batch touches, looked up once each rather than once per test.
    private async Task<Dictionary<int, List<NotificationSetting>>> GetSettingsAsync(
        List<BackflowTest> tests, CancellationToken cancellationToken)
    {
        var waterSupplierIds = tests
            .SelectMany(test => new int?[] { test.WaterSupplierId, test.WaterSupplier?.Parent?.Id })
            .Where(waterSupplierId => waterSupplierId.HasValue)
            .Select(waterSupplierId => waterSupplierId!.Value)
            .Distinct();

        var settingsByWaterSupplierId = new Dictionary<int, List<NotificationSetting>>();

        foreach (var waterSupplierId in waterSupplierIds)
        {
            var settings = await _notificationSettingService.GetCandidateSettingsAsync(waterSupplierId, cancellationToken);

            if (settings.Count > 0)
            {
                settingsByWaterSupplierId[waterSupplierId] = settings;
            }
        }

        return settingsByWaterSupplierId;
    }

    private List<NotificationMatch> MatchTest(
        BackflowTest test,
        BackflowTest? previousTest,
        Dictionary<int, List<NotificationSetting>> settingsByWaterSupplierId)
    {
        // Own water supplier's settings first, then (if any) the parent's — a parent's setting
        // fires the same way a regular one would, just attributed to the parent's WaterSupplierId.
        var matches = MatchSettings(
            test, test.WaterSupplierId, GetSettings(settingsByWaterSupplierId, test.WaterSupplierId), previousTest);

        var parentWaterSupplierId = test.WaterSupplier?.Parent?.Id;

        if (parentWaterSupplierId.HasValue)
        {
            matches.AddRange(MatchSettings(
                test, parentWaterSupplierId.Value, GetSettings(settingsByWaterSupplierId, parentWaterSupplierId.Value), previousTest));
        }

        return matches;
    }

    private static List<NotificationSetting> GetSettings(
        Dictionary<int, List<NotificationSetting>> settingsByWaterSupplierId, int waterSupplierId)
    {
        return settingsByWaterSupplierId.TryGetValue(waterSupplierId, out var settings) ? settings : [];
    }

    private List<NotificationMatch> MatchSettings(
        BackflowTest test,
        int waterSupplierId,
        List<NotificationSetting> settings,
        BackflowTest? previousTest)
    {
        var matches = new List<NotificationMatch>();

        foreach (var setting in settings)
        {
            var notification = _matcher.BuildMatch(setting, test, previousTest);

            if (notification == null)
            {
                continue;
            }

            ApplyRecordMetadata(notification, test, waterSupplierId);

            matches.Add(new NotificationMatch(notification, test, setting.User?.EmailAddress));
        }

        return matches;
    }

    private static void ApplyRecordMetadata(Notification notification, BackflowTest test, int waterSupplierId)
    {
        notification.WaterSupplierId = waterSupplierId;
        notification.ModuleType = NotificationModuleType.Backflow;
        notification.RecordId = test.Id;
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

    // One email per recipient, covering every match found for them in this run. Delayed intervals are
    // recorded only — nothing delivers them yet.
    private async Task SendImmediateEmailsAsync(List<NotificationMatch> matches, CancellationToken cancellationToken)
    {
        var recipientGroups = matches
            .Where(match => match.Notification.Interval == NotificationInterval.Immediate)
            .GroupBy(match => match.Notification.UserId);

        foreach (var recipientGroup in recipientGroups)
        {
            var emailAddress = recipientGroup
                .Select(match => match.EmailAddress)
                .FirstOrDefault(address => !string.IsNullOrWhiteSpace(address));

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                continue;
            }

            try
            {
                await SendRecipientEmailAsync(emailAddress, [.. recipientGroup], cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send the notification email for user {UserId}.", recipientGroup.Key);
            }
        }
    }

    private async Task SendRecipientEmailAsync(
        string emailAddress, List<NotificationMatch> matches, CancellationToken cancellationToken)
    {
        var vm = BuildEmailVm(matches);

        await _emailService.SendAsync(new EmailDto<BackflowTestNotificationVm>
        {
            Recipients = [emailAddress],
            Subject = BuildSubject(vm),
            TemplateId = "Notifications.BackflowTestNotification",
            TemplateData = vm
        });

        await _notificationService.MarkSentAsync(matches.Select(match => match.Notification.Id), cancellationToken);
    }

    private static BackflowTestNotificationVm BuildEmailVm(List<NotificationMatch> matches)
    {
        var groups = matches
            .GroupBy(match => match.Notification.Description)
            .OrderBy(group => group.Key)
            .Select(group => new BackflowTestNotificationGroupVm
            {
                Description = group.Key,
                Color = group.First().Notification.Color,
                Items = [.. group
                    .Select(BuildEmailItem)
                    .OrderBy(item => item.Title)
                    .ThenBy(item => item.TestId)]
            })
            .ToList();

        return new BackflowTestNotificationVm
        {
            TestCount = matches.Select(match => match.Test.Id).Distinct().Count(),
            Groups = groups
        };
    }

    private static BackflowTestNotificationItemVm BuildEmailItem(NotificationMatch match)
    {
        var serialNumber = string.IsNullOrWhiteSpace(match.Test.SerialNumber) ? null : $"#{match.Test.SerialNumber}";

        var parts = new[] { match.Notification.RecordDescription, serialNumber, match.Test.HazardType }
            .Where(part => !string.IsNullOrWhiteSpace(part));

        var details = string.Join(" · ", parts);

        return new BackflowTestNotificationItemVm
        {
            TestId = match.Test.Id,
            Title = string.IsNullOrWhiteSpace(match.Notification.PropertyDescription)
                ? $"Backflow test #{match.Test.Id}"
                : match.Notification.PropertyDescription,
            Details = string.IsNullOrEmpty(details) ? null : details,
            LocationDescription = match.Test.LocationDescription
        };
    }

    // A single match keeps the subject line it had before aggregation, so mail rules built on it keep working.
    private static string BuildSubject(BackflowTestNotificationVm vm)
    {
        if (vm.Groups.Count == 1)
        {
            var group = vm.Groups[0];

            return group.Items.Count == 1
                ? $"Envirotrax Notification - {group.Description}"
                : $"Envirotrax Notification - {group.Description} ({group.Items.Count})";
        }

        var testLabel = vm.TestCount == 1 ? "backflow test" : "backflow tests";

        return $"Envirotrax Notification - {vm.TestCount} {testLabel}";
    }

    private sealed record NotificationMatch(Notification Notification, BackflowTest Test, string? EmailAddress);
}
