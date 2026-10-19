using System.Globalization;
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Repositories.Definitions.Notifications;
using Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Templates.Emails.Notifications;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class NotificationDigestService : INotificationDigestService
{
    private static readonly NotificationInterval[] DigestIntervals =
    [
        NotificationInterval.EndOfDay,
        NotificationInterval.EndOfWeek,
        NotificationInterval.EndOfMonth
    ];

    private readonly IMapper _mapper;
    private readonly INotificationSettingRepository _settingRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailService _emailService;

    public NotificationDigestService(
        IMapper mapper,
        INotificationSettingRepository settingRepository,
        INotificationRepository notificationRepository,
        IEmailService emailService)
    {
        _mapper = mapper;
        _settingRepository = settingRepository;
        _notificationRepository = notificationRepository;
        _emailService = emailService;
    }

    public async Task<List<NotificationDigestSettingDto>> GetPendingAsync(int batchSize, DateTimeOffset asOfTime, CancellationToken cancellationToken)
    {
        var settings = new List<NotificationSetting>();

        foreach (var interval in DigestIntervals)
        {
            var remaining = batchSize - settings.Count;

            if (remaining <= 0)
            {
                break;
            }

            var period = NotificationDigestPeriod.GetLastClosed(interval, asOfTime.UtcDateTime);

            settings.AddRange(await _settingRepository.GetPendingDigestsAsync(interval, period.StartUtc, period.EndUtc, remaining, cancellationToken));
        }

        return _mapper.Map<List<NotificationDigestSettingDto>>(settings);
    }

    public async Task ScheduleAsync(int notificationSettingId, DateTimeOffset asOfTime, CancellationToken cancellationToken)
    {
        var setting = await _settingRepository.GetForDigestAsync(notificationSettingId, cancellationToken);

        if (setting == null || setting.Interval == NotificationInterval.Immediate)
        {
            return;
        }

        var period = NotificationDigestPeriod.GetLastClosed(setting.Interval, asOfTime.UtcDateTime);
        var notifications = await _notificationRepository.GetForDigestAsync(setting.UserId, setting.Interval, period.StartUtc, period.EndUtc, cancellationToken);

        var unscheduledSettingNotifications = notifications
            .Where(notification => notification.NotificationSettingId == setting.Id && notification.EmailBatchId == null)
            .ToList();

        if (unscheduledSettingNotifications.Count == 0)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;
        var email = BuildEmail(setting, period, notifications, utcNow);
        var sendTimeUtc = period.GetSendTimeUtc(utcNow);
        var batchId = await _emailService.CreateBatchIdAsync();

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        foreach (var notification in unscheduledSettingNotifications)
        {
            notification.EmailBatchId = batchId;
            notification.SentTime = sendTimeUtc;
        }

        await _notificationRepository.SaveChangesAsync();
        await _emailService.ScheduleAsync(email, batchId, new DateTimeOffset(sendTimeUtc));

        scope.Complete();
    }

    private static EmailDto<NotificationDigestVm> BuildEmail(
        NotificationSetting setting, NotificationDigestPeriod period, List<Notification> notifications, DateTime utcNow)
    {
        var vm = new NotificationDigestVm
        {
            Period = period.Description,
            DateGenerated = NotificationDigestPeriod.ToCentralTime(utcNow).ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture),
            WaterSupplierName = setting.WaterSupplier?.Name ?? string.Empty,
            Account = setting.User!.EmailAddress,
            BackflowTestType = DescribeReasonForTest(setting.ReasonForTest),
            PropertyType = DescribePropertyType(setting),
            NotificationFilter = DescribeNotificationFilter(setting),
            HazardTypes = DescribeHazardTypes(setting),
            Description = setting.Description,
            Color = setting.Color,
            Items = [.. notifications
                .Select(BuildItem)
                .OrderBy(item => item.Title)
                .ThenBy(item => item.TestId)]
        };

        return new EmailDto<NotificationDigestVm>
        {
            Recipients = [setting.User.EmailAddress],
            Subject = $"Envirotrax {DescribeInterval(setting.Interval)} Notification ({notifications.Count} notifications)",
            TemplateId = "Notifications.NotificationDigest",
            TemplateData = vm
        };
    }

    private static BackflowTestNotificationItemVm BuildItem(Notification notification)
    {
        return new BackflowTestNotificationItemVm
        {
            TestId = notification.RecordId,
            Title = string.IsNullOrWhiteSpace(notification.PropertyDescription)
                ? $"Backflow test #{notification.RecordId}"
                : notification.PropertyDescription,
            Details = notification.RecordDescription
        };
    }

    private static string DescribeInterval(NotificationInterval interval)
    {
        return interval switch
        {
            NotificationInterval.EndOfDay => "End of Day",
            NotificationInterval.EndOfWeek => "End of Week",
            _ => "End of Month"
        };
    }

    private static string DescribeReasonForTest(BackflowReasonForTest? reasonForTest)
    {
        return reasonForTest switch
        {
            null => "Any",
            BackflowReasonForTest.AnnualTest => "Annual Test",
            BackflowReasonForTest.NewInstallation => "New Installation",
            BackflowReasonForTest.ExistingInstallation => "Existing Installation",
            BackflowReasonForTest.Replacement => "Replacement",
            BackflowReasonForTest.Repair => "Repair",
            BackflowReasonForTest.AnnualTestAfterRepairs => "Annual Test After Repairs",
            _ => string.Empty
        };
    }

    private static string DescribePropertyType(NotificationSetting setting)
    {
        if (setting.PropertyTypeAny)
        {
            return "Any";
        }

        return JoinSelected(
            (setting.PropertyTypeResidential, "Residential"),
            (setting.PropertyTypeCommercial, "Commercial"));
    }

    private static string DescribeNotificationFilter(NotificationSetting setting)
    {
        if (setting.FilterAny)
        {
            return "All";
        }

        return JoinSelected(
            (setting.FilterFailedTest, "Failed Test"),
            (setting.FilterPassingTest, "Passing Test"),
            (setting.FilterUnknownSerialNumber, "Unknown Serial Number"),
            (setting.FilterInactiveProperty, "Inactive Property"),
            (setting.FilterNonCompliance, "Non-Compliance"),
            (setting.FilterPotableNonPotableMismatch, "Potable/NonPotable Mismatch"),
            (setting.FilterDuplicateTest, "Duplicate Test"),
            (setting.FilterOutOfService, "Out of Service"),
            (setting.FilterContainsRemarks, "Contains Remarks"),
            (setting.FilterBackflowNotProperlyInstalled, "Backflow not Properly Installed"),
            (setting.FilterFeeExempt, "Fee Exempt"),
            (setting.FilterHasOnSiteSewageFacility, "Has OSSF"),
            (setting.FilterHasAuxWaterSupply, "Has Aux Water Supply"),
            (setting.FilterSubmissionDaysExceeded, "Submission Days Exceeded"));
    }

    private static string DescribeHazardTypes(NotificationSetting setting)
    {
        if (setting.HazardTypeAny)
        {
            return "All";
        }

        return JoinSelected(
            (setting.HazardTypeAgriculturalFeedLot, "Agricultural/Feed Lot"),
            (setting.HazardTypeDomesticPremisesIsolation, "Domestic/Premises Isolation"),
            (setting.HazardTypeFireSystem, "Fire System"),
            (setting.HazardTypeFireHydrantTemporaryConstruction, "Fire Hydrant/Temporary Construction"),
            (setting.HazardTypeGasStationCarWash, "Gas Station/Car Wash"),
            (setting.HazardTypeIrrigationNonChemical, "Irrigation - Non Chemical"),
            (setting.HazardTypeIrrigationChemicalFeed, "Irrigation - Chemical Feed"),
            (setting.HazardTypeLaundryCleaners, "Laundry/Cleaners"),
            (setting.HazardTypeMedicalDentalLaboratoryMortuary, "Medical/Dental/Laboratory/Mortuary"),
            (setting.HazardTypeNailsSalonGrooming, "Nails/Salon/Grooming"),
            (setting.HazardTypePoolRecreationAthletics, "Pool/Recreation/Athletics"),
            (setting.HazardTypeRestaurantVendingGrocery, "Restaurant/Vending/Grocery"),
            (setting.HazardTypeFountainsGardenPondsWaterFeatures, "Fountains/Garden Ponds/Water Features"),
            (setting.HazardTypeWaterSoftener, "Water Softener"),
            (setting.HazardTypeOther, "Other"));
    }

    private static string JoinSelected(params (bool Selected, string Label)[] options)
    {
        return string.Join(", ", options.Where(option => option.Selected).Select(option => option.Label));
    }
}
