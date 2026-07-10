import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { InterceptorType } from "../../enums/interceptor-type.enum";
import { InterceptorCapacityType } from "../../enums/interceptor-capacity-type.enum";
import { FacilityType } from "../../enums/facility-type.enum";
import { PropertyType } from "../../enums/property-type.enum";
import { FogInspectionResult, FogReasonForInspection } from "../../models/fog/fog-inspection-enums";

@Injectable({
    providedIn: 'root'
})
export class FogInspectionOptionsService {
    // Interceptor / trap type — submission form (base) and search-filter variant
    public readonly interceptorTypeOptions: InputOption[] = [
        { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
        { id: InterceptorType.GritTrap, text: 'Grit Trap' },
        { id: InterceptorType.SepticTank, text: 'Septic Tank' },
        { id: InterceptorType.ChemicalToilet, text: 'Chemical Toilet' },
        { id: InterceptorType.Other, text: 'Other' }
    ];

    public readonly interceptorTypeFilterOptions: InputOption[] = [
        { id: '', text: 'Any type' },
        ...this.interceptorTypeOptions
    ];

    // Capacity type (submission form)
    public readonly capacityTypeOptions: InputOption[] = [
        { id: InterceptorCapacityType.Gallons, text: 'Gallons' },
        { id: InterceptorCapacityType.CubicYards, text: 'Cubic Yards' }
    ];

    // Reason for inspection (submission form)
    public readonly reasonOptions: InputOption[] = [
        { id: FogReasonForInspection.Scheduled, text: 'Scheduled' },
        { id: FogReasonForInspection.Unscheduled, text: 'Unscheduled' },
        { id: FogReasonForInspection.Complaint, text: 'Complaint' }
    ];

    // Sampled from (submission form)
    public readonly sampledFromOptions: InputOption[] = [
        { id: 'Inlet Chamber', text: 'Inlet Chamber' },
        { id: 'Outlet Chamber', text: 'Outlet Chamber' },
        { id: 'Sampling Well', text: 'Sampling Well' },
        { id: 'Clean-Out', text: 'Clean-Out' },
        { id: 'Outfall Tee', text: 'Outfall Tee' }
    ];

    // Facility type — submission form variant (numeric ids; bound to the FacilityType enum)
    public readonly facilityTypeOptions: InputOption[] = [
        { id: FacilityType.Restaurant, text: 'Restaurant' },
        { id: FacilityType.FastFoodEstablishment, text: 'Fast Food Establishment' },
        { id: FacilityType.HotelMotel, text: 'Hotel/Motel' },
        { id: FacilityType.CarWash, text: 'Car Wash' },
        { id: FacilityType.SchoolUniversity, text: 'School/University' },
        { id: FacilityType.GroceryStore, text: 'Grocery Store' },
        { id: FacilityType.ConvenienceStore, text: 'Convenience Store' },
        { id: FacilityType.AssistedLivingFacility, text: 'Assisted Living Facility' },
        { id: FacilityType.MedicalFacility, text: 'Medical Facility' },
        { id: FacilityType.Industrial, text: 'Industrial' },
        { id: FacilityType.CityOwnedFacility, text: 'City Owned Facility' },
        { id: FacilityType.Other, text: 'Other' }
    ];

    // Facility type — search filter variant (string ids + existing search wording/order)
    public readonly facilityTypeFilterOptions: InputOption[] = [
        { id: FacilityType.Other.toString(), text: 'Other' },
        { id: FacilityType.Restaurant.toString(), text: 'Restaurant' },
        { id: FacilityType.FastFoodEstablishment.toString(), text: 'Fast food establishment' },
        { id: FacilityType.HotelMotel.toString(), text: 'Hotel/motel' },
        { id: FacilityType.CarWash.toString(), text: 'Car wash' },
        { id: FacilityType.SchoolUniversity.toString(), text: 'School/university' },
        { id: FacilityType.GroceryStore.toString(), text: 'Grocery store' },
        { id: FacilityType.ConvenienceStore.toString(), text: 'Convenience store' },
        { id: FacilityType.AssistedLivingFacility.toString(), text: 'Assisted living facility' },
        { id: FacilityType.MedicalFacility.toString(), text: 'Medical facility' },
        { id: FacilityType.Industrial.toString(), text: 'Industrial' },
        { id: FacilityType.CityOwnedFacility.toString(), text: 'City-owned facility' }
    ];

    // Inspection result — search filter
    public readonly inspectionResultFilterOptions: InputOption[] = [
        { id: '', text: 'All results' },
        { id: FogInspectionResult.Passed.toString(), text: 'Passed' },
        { id: FogInspectionResult.Failed.toString(), text: 'Failed' }
    ];

    // Property type — search filter
    public readonly propertyTypeFilterOptions: InputOption[] = [
        { id: '', text: 'Any value' },
        { id: PropertyType.Residential.toString(), text: 'Residential' },
        { id: PropertyType.Commercial.toString(), text: 'Commercial' }
    ];
}
