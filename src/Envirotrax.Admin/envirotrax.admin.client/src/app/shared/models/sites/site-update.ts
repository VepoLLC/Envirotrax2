import { FacilityType, GreaseTrapType, PropertyType } from './site';

/**
 * Update payload sent by the Edit Site Save action to PUT /api/sites/{id}. Contains ONLY the fields the
 * Edit Site screen makes editable (Property Information, Mailing Information, Property Settings). GIS
 * values are excluded here and saved separately via SiteGisUpdate; protected fields (id, waterSupplier,
 * audit, assignments, needsRenewalCheck) are never sent — the server derives/owns those.
 *
 * Every property is required: this is a full PUT and the server's load-then-modify copies each field onto
 * the site unconditionally, so a field must always be sent. Nullable domain fields (optional strings,
 * dates and state ids) are typed `| null` and sent as null when empty; the value-type fields (booleans,
 * numbers, enums) always carry a concrete value.
 */
export interface SiteUpdate {
    // Property Information
    propertyType: PropertyType;
    businessName: string | null;
    streetNumber: string | null;
    streetName: string | null;
    propertyNumber: string | null;
    city: string | null;
    stateId: number | null;
    zipCode: string | null;

    // Mailing Information
    mailingCompanyName: string | null;
    mailingContactName: string | null;
    mailingStreetNumber: string | null;
    mailingStreetName: string | null;
    mailingNumber: string | null;
    mailingCity: string | null;
    mailingStateId: number | null;
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
 * GIS update payload sent to PUT /api/sites/{id}/gis-data. Saved through its own endpoint (GIS is
 * write-isolated from the normal Site save) and only when a GIS value actually changed. All properties
 * are required (full PUT); latitude/longitude are `number | null`. Status mirrors the GisStatusType
 * values: Error = -1, Not Set = 0, Geocoded = 1 (kept as a number, matching the client's existing GIS
 * status representation — there is no GIS status enum in the client models).
 */
export interface SiteGisUpdate {
    latitude: number | null;
    longitude: number | null;
    status: number;
}
