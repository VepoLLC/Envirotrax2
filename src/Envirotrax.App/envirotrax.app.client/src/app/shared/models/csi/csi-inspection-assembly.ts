import { BackflowTestResult } from "../backflow/backflow-test-enums";
import { CsiInspection } from "./csi-inspection";

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

// The inspection form in one request: the inspection plus its "Assemblies at this Location" tab.
export interface CreateCsiInspection extends CsiInspection {
    assemblies: CsiInspectionAssemblySelection[];
    newAssemblies: CsiInspectionNewAssembly[];
}

// A row the inspector kept: a saved row of the inspection (id) or a current test at the site (testId).
export interface CsiInspectionAssemblySelection {
    id?: number;
    testId?: number | null;
    visuallyIdentified: boolean;
}

// An assembly the inspector added on the form ("+ Add Assembly"); it is saved with the inspection.
export interface CsiInspectionNewAssembly {
    visuallyIdentified?: boolean;
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
