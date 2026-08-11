import { FacilityType, GreaseTrapType, PropertyType } from './site';

// State reference sent as `{ id }`, matching the App SiteDto's nested State/MailingState shape.
export interface SiteStateReference {
    id: number;
}

/**
 * Edit Site save payload (PUT /api/sites/{id}) — only the editable Property/Mailing/Settings fields.
 * GIS is saved separately via SiteGisUpdate; protected fields (id, waterSupplier, audit, needsRenewalCheck)
 * are never sent. state/mailingState are nested `{ id }` to match the App SiteDto.
 */
export interface SiteUpdate {
    // Property Information
    propertyType: PropertyType;
    businessName: string | null;
    streetNumber: string | null;
    streetName: string | null;
    propertyNumber: string | null;
    city: string | null;
    state: SiteStateReference | null;
    zipCode: string | null;

    // Mailing Information
    mailingCompanyName: string | null;
    mailingContactName: string | null;
    mailingStreetNumber: string | null;
    mailingStreetName: string | null;
    mailingNumber: string | null;
    mailingCity: string | null;
    mailingState: SiteStateReference | null;
    mailingZipCode: string | null;
    mailingPhoneNumber: string | null;
    mailingEmailAddress: string | null;

    // Property Settings
    accountNumber: string;
    active: boolean;
    invalidMailingAddress: boolean;
    outOfArea: boolean;
    isFeeExempt: boolean;
    bypassPropertyNumberValidation: boolean;
    backflowScheduleMonth: number;
    needsCsiInspection: boolean;
    csiRenewalDate: string | null;
    needsFogInspection: boolean;
    fogInspectionExpirationDate: string | null;
    needsFogPermit: boolean;
    fogPermitExpirationDate: string | null;
    lastTripTicketDate: string | null;
    tripTicketInterval: number;
    facilityType: FacilityType;
    greaseTrapType: GreaseTrapType;
    hasOnSiteSewageFacility: boolean;
    hasAuxWaterSupply: boolean;
    hasFireSystem: boolean;
    fireSeparateWater: boolean;
    hasGritTrap: boolean;
    hasIrrigation: boolean;
    irrigationSeparateWater: boolean;
    hasDomesticPremisesIsolation: boolean;
}

/**
 * GIS payload (PUT /api/sites/{id}/gis-data) — saved separately from the normal Site save, only when a
 * GIS value changed. latitude/longitude are `number | null`; status: Error -1 / Not Set 0 / Geocoded 1.
 */
export interface SiteGisUpdate {
    latitude: number | null;
    longitude: number | null;
    status: number;
}

/**
 * Water supplier reassignment payload (PUT /api/sites/{id}/water-supplier) — a dedicated operation, never
 * part of the normal Site save.
 */
export interface SiteWaterSupplierUpdate {
    waterSupplierId: number;
}
