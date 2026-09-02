import { BackflowReasonForTest } from "../backflow/backflow-test-enums";
import { NotificationDeliveryType } from "../../enums/notification-delivery-type.enum";
import { NotificationInterval } from "../../enums/notification-interval.enum";
import { WaterSupplierUser } from "../users/water-supplier-user";

export interface NotificationSetting {
    id?: number;

    userId?: number | null;
    user?: WaterSupplierUser | null;

    description?: string;
    color?: string;

    reasonForTest?: BackflowReasonForTest | null;

    propertyTypeResidential?: boolean;
    propertyTypeCommercial?: boolean;
    propertyTypeAny?: boolean;

    filterFailedTest?: boolean;
    filterPassingTest?: boolean;
    filterUnknownSerialNumber?: boolean;
    filterInactiveProperty?: boolean;
    filterNonCompliance?: boolean;
    filterPotableNonPotableMismatch?: boolean;
    filterDuplicateTest?: boolean;
    filterOutOfService?: boolean;
    filterContainsRemarks?: boolean;
    filterBackflowNotProperlyInstalled?: boolean;
    filterFeeExempt?: boolean;
    filterHasOnSiteSewageFacility?: boolean;
    filterHasAuxWaterSupply?: boolean;
    filterSubmissionDaysExceeded?: boolean;
    filterSubmissionDaysExceededDays?: number;
    filterAny?: boolean;

    hazardTypeAgriculturalFeedLot?: boolean;
    hazardTypeDomesticPremisesIsolation?: boolean;
    hazardTypeFireSystem?: boolean;
    hazardTypeFireHydrantTemporaryConstruction?: boolean;
    hazardTypeGasStationCarWash?: boolean;
    hazardTypeIrrigationNonChemical?: boolean;
    hazardTypeIrrigationChemicalFeed?: boolean;
    hazardTypeLaundryCleaners?: boolean;
    hazardTypeMedicalDentalLaboratoryMortuary?: boolean;
    hazardTypeNailsSalonGrooming?: boolean;
    hazardTypePoolRecreationAthletics?: boolean;
    hazardTypeRestaurantVendingGrocery?: boolean;
    hazardTypeFountainsGardenPondsWaterFeatures?: boolean;
    hazardTypeWaterSoftener?: boolean;
    hazardTypeOther?: boolean;
    hazardTypeAny?: boolean;

    interval?: NotificationInterval;
    deliveryType?: NotificationDeliveryType;
}
