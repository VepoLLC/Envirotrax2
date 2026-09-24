using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Notifications;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Notifications;

public class BackflowTestNotificationMatcher : IBackflowTestNotificationMatcher
{
    public Notification? BuildMatch(NotificationSetting setting, BackflowTest test, BackflowTest? previousTest)
    {
        var notification = new Notification
        {
            UserId = setting.UserId,
            Description = setting.Description ?? string.Empty,
            Color = setting.Color ?? string.Empty,
            ReasonForTest = setting.ReasonForTest,
            Interval = setting.Interval,
            DeliveryType = setting.DeliveryType,

            PropertyTypeAny = setting.PropertyTypeAny,
            PropertyTypeResidential = setting.PropertyTypeResidential && test.PropertyType == (int)PropertyType.Residential,
            PropertyTypeCommercial = setting.PropertyTypeCommercial && test.PropertyType == (int)PropertyType.Commercial,

            FilterAny = setting.FilterAny,
            FilterFailedTest = setting.FilterFailedTest && test.TestResult == BackflowTestResult.Fail,
            FilterPassingTest = setting.FilterPassingTest
                && test.TestResult == BackflowTestResult.Pass
                && previousTest?.TestResult == BackflowTestResult.Fail,
            FilterUnknownSerialNumber = setting.FilterUnknownSerialNumber && test.UnknownSerialNumber,
            FilterInactiveProperty = setting.FilterInactiveProperty && test.Site != null && !test.Site.Active,
            FilterPotableNonPotableMismatch = setting.FilterPotableNonPotableMismatch && test.NonPotable != test.GaugeNonPotable,
            FilterDuplicateTest = setting.FilterDuplicateTest
                && previousTest != null
                && AreReadingsEqual(test, previousTest),
            FilterContainsRemarks = setting.FilterContainsRemarks && !string.IsNullOrEmpty(test.Comments),
            FilterBackflowNotProperlyInstalled = setting.FilterBackflowNotProperlyInstalled && !test.ProperlyInstalled,
            FilterFeeExempt = setting.FilterFeeExempt && test.Site?.IsFeeExempt == true,
            FilterHasOnSiteSewageFacility = setting.FilterHasOnSiteSewageFacility && test.Site?.HasOnSiteSewageFacility == true,
            FilterHasAuxWaterSupply = setting.FilterHasAuxWaterSupply && test.Site?.HasAuxWaterSupply == true,

            HazardTypeAny = setting.HazardTypeAny,
            HazardTypeAgriculturalFeedLot = setting.HazardTypeAgriculturalFeedLot && test.HazardType == "Agricultural/Feed Lot",
            HazardTypeDomesticPremisesIsolation = setting.HazardTypeDomesticPremisesIsolation && test.HazardType == "Domestic/Premises Isolation",
            HazardTypeFireSystem = setting.HazardTypeFireSystem && test.HazardType == "Fire System",
            HazardTypeFireHydrantTemporaryConstruction = setting.HazardTypeFireHydrantTemporaryConstruction && test.HazardType == "Fire Hydrant/Temporary Construction",
            HazardTypeGasStationCarWash = setting.HazardTypeGasStationCarWash && test.HazardType == "Gas Station/Car Wash",
            HazardTypeIrrigationNonChemical = setting.HazardTypeIrrigationNonChemical && test.HazardType == "Irrigation - Non Chemical",
            HazardTypeIrrigationChemicalFeed = setting.HazardTypeIrrigationChemicalFeed && test.HazardType == "Irrigation - Chemical Feed",
            HazardTypeLaundryCleaners = setting.HazardTypeLaundryCleaners && test.HazardType == "Laundry/Cleaners",
            HazardTypeMedicalDentalLaboratoryMortuary = setting.HazardTypeMedicalDentalLaboratoryMortuary && test.HazardType == "Medical/Dental/Laboratory/Mortuary",
            HazardTypeNailsSalonGrooming = setting.HazardTypeNailsSalonGrooming && test.HazardType == "Nails/Salon/Grooming",
            HazardTypePoolRecreationAthletics = setting.HazardTypePoolRecreationAthletics && test.HazardType == "Pool/Recreation/Athletics",
            HazardTypeRestaurantVendingGrocery = setting.HazardTypeRestaurantVendingGrocery && test.HazardType == "Restaurant/Vending/Grocery",
            HazardTypeFountainsGardenPondsWaterFeatures = setting.HazardTypeFountainsGardenPondsWaterFeatures && test.HazardType == "Fountains/Garden Ponds/Water Features",
            HazardTypeWaterSoftener = setting.HazardTypeWaterSoftener && test.HazardType == "Water Softener",
            HazardTypeOther = setting.HazardTypeOther && test.HazardType == "Other"
        };

        if (setting.FilterSubmissionDaysExceeded
            && test.TestDate.HasValue
            && test.TestDate.Value < DateTime.UtcNow.AddDays(-setting.FilterSubmissionDaysExceededDays))
        {
            notification.FilterSubmissionDaysExceeded = true;
            notification.FilterSubmissionDaysExceededDays = setting.FilterSubmissionDaysExceededDays;
        }

        var reasonForTestMatch = setting.ReasonForTest == null || setting.ReasonForTest == test.ReasonForTest;

        var propertyTypeMatch = notification.PropertyTypeAny
            || notification.PropertyTypeResidential
            || notification.PropertyTypeCommercial;

        var filterMatch = notification.FilterAny
            || notification.FilterFailedTest
            || notification.FilterPassingTest
            || notification.FilterUnknownSerialNumber
            || notification.FilterInactiveProperty
            || notification.FilterPotableNonPotableMismatch
            || notification.FilterDuplicateTest
            || notification.FilterContainsRemarks
            || notification.FilterBackflowNotProperlyInstalled
            || notification.FilterFeeExempt
            || notification.FilterHasOnSiteSewageFacility
            || notification.FilterHasAuxWaterSupply
            || notification.FilterSubmissionDaysExceeded;

        var hazardTypeMatch = notification.HazardTypeAny
            || notification.HazardTypeAgriculturalFeedLot
            || notification.HazardTypeDomesticPremisesIsolation
            || notification.HazardTypeFireSystem
            || notification.HazardTypeFireHydrantTemporaryConstruction
            || notification.HazardTypeGasStationCarWash
            || notification.HazardTypeIrrigationNonChemical
            || notification.HazardTypeIrrigationChemicalFeed
            || notification.HazardTypeLaundryCleaners
            || notification.HazardTypeMedicalDentalLaboratoryMortuary
            || notification.HazardTypeNailsSalonGrooming
            || notification.HazardTypePoolRecreationAthletics
            || notification.HazardTypeRestaurantVendingGrocery
            || notification.HazardTypeFountainsGardenPondsWaterFeatures
            || notification.HazardTypeWaterSoftener
            || notification.HazardTypeOther;

        if (!reasonForTestMatch || !propertyTypeMatch || !filterMatch || !hazardTypeMatch)
        {
            return null;
        }

        return notification;
    }

    // Only the primary assembly's readings — the bypass assembly ("...2" fields) is out of scope.
    private static bool AreReadingsEqual(BackflowTest test, BackflowTest previousTest)
    {
        return test.TestResult == previousTest.TestResult
            && test.InitCV1HeldPSID == previousTest.InitCV1HeldPSID
            && test.InitCV1ClosedTight == previousTest.InitCV1ClosedTight
            && test.InitCV1Leaked == previousTest.InitCV1Leaked
            && test.InitCV2HeldPSID == previousTest.InitCV2HeldPSID
            && test.InitCV2ClosedTight == previousTest.InitCV2ClosedTight
            && test.InitCV2Leaked == previousTest.InitCV2Leaked
            && test.InitRVOpenedPSID == previousTest.InitRVOpenedPSID
            && test.InitRVDidNotOpen == previousTest.InitRVDidNotOpen
            && test.InitBCHeldPSID == previousTest.InitBCHeldPSID
            && test.InitBCClosedTight == previousTest.InitBCClosedTight
            && test.InitBCLeaked == previousTest.InitBCLeaked
            && test.InitPvbAirInletOpenedPSID == previousTest.InitPvbAirInletOpenedPSID
            && test.InitPvbAirInletDidNotOpen == previousTest.InitPvbAirInletDidNotOpen
            && test.InitPvbAirInletFullyOpened == previousTest.InitPvbAirInletFullyOpened
            && test.InitPvbCVHeldPSID == previousTest.InitPvbCVHeldPSID
            && test.InitPvbCVLeaked == previousTest.InitPvbCVLeaked
            && test.AirGapValid == previousTest.AirGapValid
            && test.FinalCV1HeldPSID == previousTest.FinalCV1HeldPSID
            && test.FinalCV1ClosedTight == previousTest.FinalCV1ClosedTight
            && test.FinalCV2HeldPSID == previousTest.FinalCV2HeldPSID
            && test.FinalCV2ClosedTight == previousTest.FinalCV2ClosedTight
            && test.FinalRVOpenedPSID == previousTest.FinalRVOpenedPSID
            && test.FinalBCHeldPSID == previousTest.FinalBCHeldPSID
            && test.FinalBCClosedTight == previousTest.FinalBCClosedTight
            && test.FinalPvbAirInletOpenedPSID == previousTest.FinalPvbAirInletOpenedPSID
            && test.FinalPvbAirInletFullyOpened == previousTest.FinalPvbAirInletFullyOpened
            && test.FinalPvbCVHeldPSID == previousTest.FinalPvbCVHeldPSID;
    }
}
