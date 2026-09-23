import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { WaterSupplier } from '../water-suppliers/water-supplier';

export enum CsiPaymentStatus {
    Paid = 1,
    Unpaid = 2
}

export enum CsiInspectionReason {
    NewConstruction = 0,
    ExistingServiceContaminantHazardsSuspected = 1,
    MajorRenovationOrExpansion = 2
}

export const csiInspectionReasonLabels: Record<CsiInspectionReason, string> = {
    [CsiInspectionReason.NewConstruction]: 'New construction',
    [CsiInspectionReason.ExistingServiceContaminantHazardsSuspected]: 'Existing service where contaminant hazards are suspected',
    [CsiInspectionReason.MajorRenovationOrExpansion]: 'Major renovation or expansion of distribution facilities'
};

export enum BackflowTestResult {
    Pass = 0,
    Fail = 1,
    PassAfterRepairs = 2
}

export class CsiInspection {
    id?: number;
    inspectionDate?: string;
    inspectionResult?: boolean;
    transactionId?: string;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;
    inspectorCompanyName?: string;
    inspectorContactName?: string;
    inspectorLicenseType?: string;
    inspectorLicenseNumber?: string;
}

export class CsiInspectionDetails {
    id?: number;
    waterSupplier?: WaterSupplier;
    inspectorUser?: { id?: number; emailAddress?: string; contactName?: string };
    inspectionDate?: string;
    submissionId?: string;

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
    mailingPhoneNumber?: string;
    mailingEmailAddress?: string;

    inspectorCompanyName?: string;
    inspectorJobTitle?: string;
    inspectorContactName?: string;
    inspectorAddress?: string;
    inspectorCity?: string;
    inspectorState?: string;
    inspectorZip?: string;
    inspectorWorkNumber?: string;
    inspectorCellNumber?: string;
    inspectorFaxNumber?: string;
    inspectorLicenseNumber?: string;
    inspectorLicenseType?: string;

    reasonForInspection?: CsiInspectionReason;

    compliance1?: boolean;
    compliance2?: boolean;
    compliance3?: boolean;
    compliance4?: boolean;
    compliance5?: boolean;
    compliance6?: boolean;

    materialServiceLineLead?: boolean;
    materialServiceLineCopper?: boolean;
    materialServiceLinePVC?: boolean;
    materialServiceLineOther?: boolean;
    materialServiceLineOtherDescription?: string;

    materialSolderLead?: boolean;
    materialSolderLeadFree?: boolean;
    materialSolderSolventWeld?: boolean;
    materialSolderOther?: boolean;
    materialSolderOtherDescription?: string;

    aiOssf?: boolean;
    aiWaterWell?: boolean;
    aiFireSystem?: boolean;
    aiFireSystem2?: boolean;
    aiGreaseTrap?: boolean;
    aiSandGrit?: boolean;
    aiReclaimedWater?: boolean;
    aiIrrigationSystem?: boolean;
    aiIrrigationSystem2?: boolean;

    inspectionResult?: boolean;
    disapproved?: boolean;
    disapprovedReason?: string;

    comments?: string;

    transactionId?: string;
    transactionDate?: string;
}

export class CsiInspectionAssembly {
    id?: number;
    inspectionId?: number;
    testId?: number;
    visuallyIdentified?: boolean;
    deviceType?: string;
    assemblyDescription?: string;
    serialNumber?: string;
    assemblyDescription2?: string;
    serialNumber2?: string;
    hazardType?: string;
    hazardTypeOtherDescription?: string;
    locationDescription?: string;
    isCurrent?: boolean;
    testResult?: BackflowTestResult;
    outOfService?: boolean;
    testDate?: string;
    expirationDate?: string;
    transactionId?: string;
    disapproved?: boolean;
    rejected?: boolean;
}

export class CsiInspectionImage {
    id?: number;
    inspectionId?: number;
    description?: string;
    url?: string;
}

export class CsiInspectionCounts {
    assemblyCount?: number;
    recordLogCount?: number;
}
