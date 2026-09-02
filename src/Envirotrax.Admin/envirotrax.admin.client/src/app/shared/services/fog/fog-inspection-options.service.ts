import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import {
    FogInspectionResult,
    FogPaymentStatus,
    FogTotalCapacityRange,
    InterceptorType
} from "../../models/fog/fog-inspection";
import { FacilityType, PropertyType } from "../../models/sites/site";

@Injectable({
    providedIn: 'root'
})
export class FogInspectionOptionsService {
    public readonly facilityTypeOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        { id: String(FacilityType.Restaurant), text: 'Restaurant' },
        { id: String(FacilityType.FastFoodEstablishment), text: 'Fast Food Establishment' },
        { id: String(FacilityType.HotelMotel), text: 'Hotel/Motel' },
        { id: String(FacilityType.CarWash), text: 'Car Wash' },
        { id: String(FacilityType.SchoolUniversity), text: 'School/University' },
        { id: String(FacilityType.GroceryStore), text: 'Grocery Store' },
        { id: String(FacilityType.ConvenienceStore), text: 'Convenience Store' },
        { id: String(FacilityType.AssistedLivingFacility), text: 'Assisted Living Facility' },
        { id: String(FacilityType.MedicalFacility), text: 'Medical Facility' },
        { id: String(FacilityType.Industrial), text: 'Industrial' },
        { id: String(FacilityType.CityOwnedFacility), text: 'City Owned Facility' },
        { id: String(FacilityType.Other), text: 'Other' }
    ];

    public readonly interceptorTypeOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public readonly totalCapacityOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FogTotalCapacityRange.TwentyFivePercentOrLess), text: '25% or less' },
        { id: String(FogTotalCapacityRange.GreaterThanTwentyFivePercent), text: 'Greater than 25%' }
    ];

    public readonly inspectionResultOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FogInspectionResult.Passed), text: 'Pass' },
        { id: String(FogInspectionResult.Failed), text: 'Fail' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FogPaymentStatus.Paid), text: 'Paid' },
        { id: String(FogPaymentStatus.Unpaid), text: 'Unpaid' }
    ];

    public readonly propertyTypeOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public readonly dateSearchOptions: InputOption[] = [
        { id: '', text: 'None' },
        { id: 'inspectionDate', text: 'Inspection Date' },
        { id: 'createdTime', text: 'Record Creation Date' }
    ];
}
