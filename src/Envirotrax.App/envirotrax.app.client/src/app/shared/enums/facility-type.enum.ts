export enum FacilityType {
    Other = 0,
    Restaurant = 1,
    FastFoodEstablishment = 2,
    HotelMotel = 3,
    CarWash = 4,
    SchoolUniversity = 5,
    GroceryStore = 6,
    ConvenienceStore = 7,
    AssistedLivingFacility = 8,
    MedicalFacility = 9,
    Industrial = 10,
    CityOwnedFacility = 11
}

export const facilityTypeLabels: Record<FacilityType, string> = {
    [FacilityType.Other]: 'Other',
    [FacilityType.Restaurant]: 'Restaurant',
    [FacilityType.FastFoodEstablishment]: 'Fast food establishment',
    [FacilityType.HotelMotel]: 'Hotel/motel',
    [FacilityType.CarWash]: 'Car wash',
    [FacilityType.SchoolUniversity]: 'School/university',
    [FacilityType.GroceryStore]: 'Grocery store',
    [FacilityType.ConvenienceStore]: 'Convenience store',
    [FacilityType.AssistedLivingFacility]: 'Assisted living facility',
    [FacilityType.MedicalFacility]: 'Medical facility',
    [FacilityType.Industrial]: 'Industrial',
    [FacilityType.CityOwnedFacility]: 'City-owned facility'
};
