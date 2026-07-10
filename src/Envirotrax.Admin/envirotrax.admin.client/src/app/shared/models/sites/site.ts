export enum PropertyType {
    Residential = 0,
    Commercial = 1
}

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

export enum GreaseTrapType {
    TrapNotRequired = 0,
    HasGreaseTrap = 1,
    ShouldHaveGreaseTrap = 2,
    MightHaveGreaseTrap = 3
}

export enum FogCompliancyStatus {
    Compliant = 1,
    OutOfCompliance = 2
}

export class Site {
    id?: number;
    waterSupplier?: { id?: number; name?: string };
    accountNumber?: string;
    businessName?: string;
    propertyType?: PropertyType;
    streetNumber?: string;
    streetName?: string;
    propertyNumber?: string;
    city?: string;
    state?: { id?: number; name?: string; code?: string };
    active?: boolean;
    outOfArea?: boolean;
    isFeeExempt?: boolean;
}
