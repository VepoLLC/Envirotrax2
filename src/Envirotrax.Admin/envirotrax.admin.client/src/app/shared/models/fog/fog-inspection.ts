import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';

export enum FogInspectionResult {
    Passed = 0,
    Failed = 1
}

export enum InterceptorType {
    GreaseTrap = 'Grease Trap',
    GritTrap = 'Grit Trap',
    SepticTank = 'Septic Tank',
    ChemicalToilet = 'Chemical Toilet',
    Other = 'Other'
}

export enum InterceptorCapacityType {
    Gallons = 0,
    CubicYards = 1
}

export const interceptorCapacityTypeLabels: Record<InterceptorCapacityType, string> = {
    [InterceptorCapacityType.Gallons]: 'Gallons',
    [InterceptorCapacityType.CubicYards]: 'Cubic Yards'
};

export enum FogReasonForInspection {
    Scheduled = 0,
    Unscheduled = 1,
    Complaint = 2
}

export const fogReasonForInspectionLabels: Record<FogReasonForInspection, string> = {
    [FogReasonForInspection.Scheduled]: 'Scheduled',
    [FogReasonForInspection.Unscheduled]: 'Unscheduled',
    [FogReasonForInspection.Complaint]: 'Complaint'
};

/** Mirrors the server ReferencedWaterSupplierDto - only the members this window renders. */
export interface ReferencedWaterSupplier {
    id?: number;
    name?: string;
    contactName?: string;
    pwsId?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    phoneNumber?: string;
    emailAddress?: string;
}

export interface ReferencedFogSite {
    id?: number;
    accountNumber?: string;
}

/** Mirrors the server ReferencedProfessionalUserDto. */
export interface ReferencedProfessionalUser {
    id?: number;
    emailAddress?: string;
    contactName?: string;
}

export class FogInspection {
    id?: number;
    inspectionDate?: string;
    inspectionResult?: FogInspectionResult;
    transactionId?: string;
    totalCapacityPercent?: number;

    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;

    interceptorType?: string;
    interceptorOtherDescription?: string;
    interceptorCapacity?: number;
    interceptorCapacityType?: InterceptorCapacityType;
    interceptorLocationDescription?: string;

    inspectorCompanyName?: string;
    inspectorContactName?: string;

    createdTime?: string;

    // Detail members - populated by GET api/fog/inspections/{id}, absent on search rows.
    waterSupplier?: ReferencedWaterSupplier;
    site?: ReferencedFogSite;
    submissionId?: string;

    mailingCompanyName?: string;
    mailingContactName?: string;
    mailingStreetNumber?: string;
    mailingStreetName?: string;
    mailingNumber?: string;
    mailingCity?: string;
    mailingState?: State;
    mailingZip?: string;
    mailingPhoneNumber?: string;
    mailingEmailAddress?: string;

    inspector?: ReferencedProfessionalUser;
    inspectorJobTitle?: string;
    inspectorAddress?: string;
    inspectorCity?: string;
    inspectorState?: string;
    inspectorZip?: string;
    inspectorWorkNumber?: string;
    inspectorCellNumber?: string;
    inspectorFaxNumber?: string;

    fogGeneratorPhoneNumber?: string;
    fogGeneratorEmailAddress?: string;
    facilityType?: number;
    reasonForInspection?: FogReasonForInspection;

    interceptorComments?: string;

    maintained?: boolean;
    accessible?: boolean;
    pastOverflow?: boolean;

    inletChamberWettingHeight?: string;
    inletChamberGreaseBlanket?: string;
    inletChamberSediments?: string;
    outletChamberWettingHeight?: string;
    outletChamberGreaseBlanket?: string;
    outletChamberSediments?: string;

    inletTeeIntact?: boolean;
    outletTeeIntact?: boolean;
    inletTeeVisible?: boolean;
    outletTeeVisible?: boolean;

    sampledFrom?: string;
    samplingPointAccessible?: boolean;
    samplingPointClean?: boolean;

    inletTotalCapacityPercent?: number;
    outletTotalCapacityPercent?: number;

    signatureContactName?: string;
    signatureDate?: string;
    comments?: string;

    exteriorImageUrl?: string;
    interiorImageUrl?: string;
    signatureImageUrl?: string;
}

/**
 * Search result row: display strings are pre-computed once after each load so the
 * templates only interpolate plain fields.
 */
export interface FogInspectionRow extends FogInspection {
    propertyAddress: string;
    propertyCityStateZip: string;
    interceptorDescription: string;
}

export interface FogInspectionUpdateRequest {
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;

    mailingCompanyName?: string;
    mailingContactName?: string;
    mailingStreetNumber?: string;
    mailingStreetName?: string;
    mailingNumber?: string;
    mailingCity?: string;
    mailingState?: State;
    mailingZip?: string;

    interceptorType?: string;
    interceptorOtherDescription?: string;
    interceptorCapacity?: number;
    interceptorCapacityType?: InterceptorCapacityType;
    interceptorLocationDescription?: string;

    inspectionDate?: string;
    reasonForInspection?: FogReasonForInspection;
    facilityType?: number;
    maintained?: boolean;
    accessible?: boolean;
    pastOverflow?: boolean;
    samplingPointAccessible?: boolean;
    samplingPointClean?: boolean;
    sampledFrom?: string;
    inletTeeIntact?: boolean;
    outletTeeIntact?: boolean;
    inletChamberWettingHeight?: string;
    inletChamberGreaseBlanket?: string;
    inletChamberSediments?: string;
    outletChamberWettingHeight?: string;
    outletChamberGreaseBlanket?: string;
    outletChamberSediments?: string;
    inletTotalCapacityPercent?: number;
    outletTotalCapacityPercent?: number;
    totalCapacityPercent?: number;
    inspectionResult?: FogInspectionResult;
    comments?: string;
}
