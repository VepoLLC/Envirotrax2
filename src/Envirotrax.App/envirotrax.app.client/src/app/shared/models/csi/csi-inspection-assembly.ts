import { BackflowTestResult } from "../backflow/backflow-test-enums";

// A visually identified assembly row: a snapshot of the backflow test as it stood at inspection time.
export interface CsiInspectionAssembly {
    id?: number;
    inspectionId?: number | null;
    testId?: number | null;
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

// An assembly the inspector adds on the inspection form ("+ Add Assembly").
export interface CsiInspectionAssemblyRequest {
    submissionId: string;
    siteId: number;
    deviceType?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    serialNumber?: string;
    manufacturer2?: string;
    model2?: string;
    size2?: string;
    serialNumber2?: string;
    hazardType?: string;
    hazardTypeOtherDescription?: string;
    locationDescription?: string;
    comments?: string;
}
