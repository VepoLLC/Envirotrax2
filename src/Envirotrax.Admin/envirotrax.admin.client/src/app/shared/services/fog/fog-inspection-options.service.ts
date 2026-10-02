import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import {
    FogInspectionResult,
    fogReasonForInspectionLabels,
    FogReasonForInspection,
    InterceptorCapacityType,
    InterceptorType
} from "../../models/fog/fog-inspection";
import { FacilityType, PropertyType } from "../../models/sites/site";

@Injectable({
    providedIn: 'root'
})
export class FogInspectionOptionsService {
    public readonly facilityTypeOptions: InputOption[] = [
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

    public readonly facilityTypeFilterOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        ...this.facilityTypeOptions
    ];

    public readonly reasonOptions: InputOption[] = [
        {
            id: String(FogReasonForInspection.Scheduled),
            text: fogReasonForInspectionLabels[FogReasonForInspection.Scheduled]
        },
        {
            id: String(FogReasonForInspection.Unscheduled),
            text: fogReasonForInspectionLabels[FogReasonForInspection.Unscheduled]
        },
        {
            id: String(FogReasonForInspection.Complaint),
            text: fogReasonForInspectionLabels[FogReasonForInspection.Complaint]
        }
    ];

    public readonly sampledFromOptions: InputOption[] = [
        { id: 'Inlet Chamber', text: 'Inlet Chamber' },
        { id: 'Outlet Chamber', text: 'Outlet Chamber' },
        { id: 'Sampling Well', text: 'Sampling Well' },
        { id: 'Clean-Out', text: 'Clean-Out' },
        { id: 'Outfall Tee', text: 'Outfall Tee' }
    ];

    public readonly interceptorTypeOptions: InputOption[] = [
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public readonly interceptorTypeFilterOptions: InputOption[] = [
        { id: '', text: 'Any Type' },
        ...this.interceptorTypeOptions
    ];

    public readonly capacityTypeOptions: InputOption[] = [
        { id: InterceptorCapacityType.Gallons, text: 'Gallons' },
        { id: InterceptorCapacityType.CubicYards, text: 'Cubic Yards' }
    ];

    public readonly totalCapacityOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'lte25', text: '25% or less' },
        { id: 'gt25', text: 'Greater than 25%' }
    ];

    public readonly inspectionResultOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: String(FogInspectionResult.Passed), text: 'Pass' },
        { id: String(FogInspectionResult.Failed), text: 'Fail' }
    ];

    public readonly paymentStatusOptions: InputOption[] = [
        { id: '', text: 'Any Value' },
        { id: 'paid', text: 'Paid' },
        { id: 'unpaid', text: 'Unpaid' }
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
