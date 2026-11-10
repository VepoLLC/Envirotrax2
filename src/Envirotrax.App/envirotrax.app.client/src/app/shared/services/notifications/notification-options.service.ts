import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { NotificationDeliveryType } from "../../enums/notification-delivery-type.enum";
import { NotificationInterval } from "../../enums/notification-interval.enum";
import { NotificationSetting } from "../../models/notifications/notification-setting";
import { BackflowTestOptionsService } from "../backflow/backflow-test-options.service";

export interface NotificationFlagOption {
    field: keyof NotificationSetting;
    text: string;
}

@Injectable({
    providedIn: 'root'
})
export class NotificationOptionsService {
    public readonly reasonForTestOptions: InputOption[];

    public readonly propertyTypeOptions: NotificationFlagOption[] = [
        { field: 'propertyTypeResidential', text: 'Residential' },
        { field: 'propertyTypeCommercial', text: 'Commercial' }
    ];

    public readonly filterOptions: NotificationFlagOption[] = [
        { field: 'filterFailedTest', text: 'Failed Test' },
        { field: 'filterPassingTest', text: 'Passing Test' },
        { field: 'filterUnknownSerialNumber', text: 'Unknown Serial Number' },
        { field: 'filterInactiveProperty', text: 'Inactive Property' },
        { field: 'filterNonCompliance', text: 'Non-Compliance' },
        { field: 'filterPotableNonPotableMismatch', text: 'Potable/NonPotable Mismatch' },
        { field: 'filterDuplicateTest', text: 'Duplicate Test Results' },
        { field: 'filterOutOfService', text: 'Out of Service' },
        { field: 'filterContainsRemarks', text: 'Contains Remarks' },
        { field: 'filterBackflowNotProperlyInstalled', text: 'Backflow Not Properly Installed' },
        { field: 'filterFeeExempt', text: 'Fee Exempt' },
        { field: 'filterHasOnSiteSewageFacility', text: 'Has OSSF' },
        { field: 'filterHasAuxWaterSupply', text: 'Has Aux Water Supply' },
        { field: 'filterSubmissionDaysExceeded', text: 'Submission Days Exceeded' }
    ];

    public readonly hazardTypeOptions: NotificationFlagOption[] = [
        { field: 'hazardTypeAgriculturalFeedLot', text: 'Agricultural/Feed Lot' },
        { field: 'hazardTypeDomesticPremisesIsolation', text: 'Domestic/Premises Isolation' },
        { field: 'hazardTypeFireSystem', text: 'Fire System' },
        { field: 'hazardTypeFireHydrantTemporaryConstruction', text: 'Fire Hydrant/Temporary Construction' },
        { field: 'hazardTypeGasStationCarWash', text: 'Gas Station/Car Wash' },
        { field: 'hazardTypeIrrigationNonChemical', text: 'Irrigation - Non Chemical' },
        { field: 'hazardTypeIrrigationChemicalFeed', text: 'Irrigation - Chemical Feed' },
        { field: 'hazardTypeLaundryCleaners', text: 'Laundry/Cleaners' },
        { field: 'hazardTypeMedicalDentalLaboratoryMortuary', text: 'Medical/Dental/Laboratory/Mortuary' },
        { field: 'hazardTypeNailsSalonGrooming', text: 'Nails/Salon/Grooming' },
        { field: 'hazardTypePoolRecreationAthletics', text: 'Pool/Recreation/Athletics' },
        { field: 'hazardTypeRestaurantVendingGrocery', text: 'Restaurant/Vending/Grocery' },
        { field: 'hazardTypeFountainsGardenPondsWaterFeatures', text: 'Fountains/Garden Ponds/Water Features' },
        { field: 'hazardTypeWaterSoftener', text: 'Water Softener' },
        { field: 'hazardTypeOther', text: 'Other' }
    ];

    public readonly intervalOptions: InputOption[] = [
        { id: NotificationInterval.Immediate, text: 'Immediate' },
        { id: NotificationInterval.EndOfDay, text: 'End of Day' },
        { id: NotificationInterval.EndOfWeek, text: 'End of Week' },
        { id: NotificationInterval.EndOfMonth, text: 'End of Month' }
    ];

    public readonly deliveryTypeOptions: InputOption[] = [
        { id: NotificationDeliveryType.Email, text: 'Email' }
    ];

    public readonly colorOptions: string[] = [
        '#ffffff', '#e0e0e0',
        '#ff0000', '#ff33cc', '#cc33ff',
        '#0000ff', '#00ccff', '#00ffcc',
        '#00ff00', '#ffff00', '#ff9900'
    ];

    constructor(backflowOptions: BackflowTestOptionsService) {
        this.reasonForTestOptions = [
            { id: null, text: 'Any' },
            ...backflowOptions.reasonOptions
        ];
    }

    public getReasonForTestText(reasonForTest?: number | null): string {
        const option = this.reasonForTestOptions.find(o => o.id === (reasonForTest ?? null));
        return option?.text ?? '';
    }

    public getIntervalText(interval?: NotificationInterval): string {
        const option = this.intervalOptions.find(o => o.id === interval);
        return option?.text ?? '';
    }

    public getDeliveryTypeText(deliveryType?: NotificationDeliveryType): string {
        const option = this.deliveryTypeOptions.find(o => o.id === deliveryType);
        return option?.text ?? '';
    }

    public getPropertyTypeText(setting: NotificationSetting): string {
        if (setting.propertyTypeAny || (setting.propertyTypeResidential && setting.propertyTypeCommercial)) {
            return 'All';
        }

        if (setting.propertyTypeResidential) {
            return 'Residential';
        }

        return setting.propertyTypeCommercial ? 'Commercial' : '';
    }

    public getFiltersText(setting: NotificationSetting): string {
        return setting.filterAny ? 'All' : this.getCheckedText(setting, this.filterOptions);
    }

    public getHazardTypesText(setting: NotificationSetting): string {
        return setting.hazardTypeAny ? 'All' : this.getCheckedText(setting, this.hazardTypeOptions);
    }

    private getCheckedText(setting: NotificationSetting, options: NotificationFlagOption[]): string {
        return options
            .filter(option => setting[option.field])
            .map(option => option.text)
            .join(', ');
    }
}
