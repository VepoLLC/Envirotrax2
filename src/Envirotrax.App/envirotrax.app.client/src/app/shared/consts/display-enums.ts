import { FacilityType } from "../enums/facility-type.enum";

export const FacilityTypeDisplay: Record<FacilityType, string> = {
    [FacilityType.Other]: 'Other',
    [FacilityType.Restaurant]: 'Restaurant',
    [FacilityType.FastFoodEstablishment]: 'Fast Food Establishment',
    [FacilityType.HotelMotel]: 'Hotel/Motel',
    [FacilityType.CarWash]: 'Car Wash',
    [FacilityType.SchoolUniversity]: 'School/University',
    [FacilityType.GroceryStore]: 'Grocery Store',
    [FacilityType.ConvenienceStore]: 'Convenience Store',
    [FacilityType.AssistedLivingFacility]: 'Assisted Living Facility',
    [FacilityType.MedicalFacility]: 'Medical Facility',
    [FacilityType.Industrial]: 'Industrial',
    [FacilityType.CityOwnedFacility]: 'City Owned Facility'
};
