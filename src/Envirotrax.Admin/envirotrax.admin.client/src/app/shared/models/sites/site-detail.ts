import { State } from '../lookup/state';
import { WaterSupplier } from '../water-suppliers/water-supplier';
import { FacilityType, GreaseTrapType, PropertyType } from './site';

/**
 * Full read/detail projection of a Site returned by GET /api/sites/{id}, consumed by the Edit Site
 * view. Mirrors the Envirotrax.App SiteDto shape so every Edit Site section can bind to it. The editable
 * sections bind to a working copy of this model, and the SiteUpdate save payload is derived from it.
 */
export interface SiteDetail {
    id?: number;
    waterSupplier?: WaterSupplier;
    subArea?: string;
    accountNumber?: string;
    businessName?: string;
    propertyType?: PropertyType;
    streetNumber?: string;
    streetName?: string;
    propertyNumber?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    mailingCompanyName?: string;
    mailingContactName?: string;
    mailingStreetNumber?: string;
    mailingStreetName?: string;
    mailingNumber?: string;
    mailingCity?: string;
    mailingState?: State;
    mailingZipCode?: string;
    mailingPhoneNumber?: string;
    mailingEmailAddress?: string;
    fogGeneratorPhoneNumber?: string;
    fogGeneratorEmailAddress?: string;
    comments?: string;
    needsCsiInspection?: boolean;
    csiRenewalDate?: string;
    needsBackflowLetter?: boolean;
    backflowLetterDate?: string;
    needsFogInspection?: boolean;
    fogInspectionExpirationDate?: string;
    needsFogPermit?: boolean;
    fogPermitExpirationDate?: string;
    lastTripTicketDate?: string;
    tripTicketInterval?: number;
    isFeeExempt?: boolean;
    rainFreezeSensorType?: number;
    hasKnownBackflowAssemblies?: boolean;
    hasOnSiteSewageFacility?: boolean;
    hasWaterWell?: boolean;
    hasAuxWaterSupply?: boolean;
    hasFireSystem?: boolean;
    fireSeparateWater?: boolean;
    greaseTrapType?: GreaseTrapType;
    hasGritTrap?: boolean;
    hasReclaimed?: boolean;
    hasIrrigation?: boolean;
    irrigationSeparateWater?: boolean;
    hasDomesticPremisesIsolation?: boolean;
    requiresDomesticPremisesIsolation?: boolean;
    invalidMailingAddress?: boolean;
    outOfArea?: boolean;
    facilityType?: FacilityType;
    backflowScheduleMonth?: number;
    gisLatitude?: number;
    gisLongitude?: number;
    gisStatus?: number;
    gisDate?: string;
    gisAreaId?: number;
    gisOutOfArea?: boolean;
    gisOutOfAreaCheckDate?: string;
    customData1?: string;
    customBooleanData1?: boolean;
    bypassPropertyNumberValidation?: boolean;
    needsRenewalCheck?: boolean;
    active?: boolean;
    createdTime?: string;
    updatedTime?: string;
    updatedBy?: { id?: number; email?: string };
}

/**
 * Model passed into the Edit Site window, carried from the Property Search row. siteId drives the load
 * (GET /api/sites/{id}) and targets the site by its globally-unique Id on save. waterSupplierId is the
 * tenant every write sends to the App API (as ?waterSupplierId=); it is optional here because the loaded
 * site detail carries the authoritative value, which also keeps it correct after a supplier reassignment.
 */
export interface SiteEditWindowModel {
    siteId: number;
    waterSupplierId?: number;
}
