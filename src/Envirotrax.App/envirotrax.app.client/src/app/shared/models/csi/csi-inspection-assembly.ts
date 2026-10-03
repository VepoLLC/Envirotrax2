import { BackflowTestResult } from "../backflow/backflow-test-enums";

// A visually identified assembly row: a snapshot of the backflow test as it stood at inspection time.
export interface CsiInspectionAssembly {
    id?: number;
    inspectionId?: number;
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

// One row of the list sent when the submission is saved. `id` marks a saved row, `testId` alone a
// current test at the site, and neither an assembly the inspector added (which carries the device fields).
export interface CsiInspectionAssemblyRequest {
    id?: number;
    testId?: number | null;
    visuallyIdentified: boolean;
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
