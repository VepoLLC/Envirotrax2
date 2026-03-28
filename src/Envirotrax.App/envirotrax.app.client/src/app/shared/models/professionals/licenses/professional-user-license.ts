import { ProfessionalLicenseType } from "./professional-license-type";

export interface ProfessionalUserLicense {
    id?: number;
    user?: { id?: number };
    professionalType?: ProfessionalType;
    licenseType?: ProfessionalLicenseType;
    licenseNumber?: string;
    expirationDate?: string;
}

export enum ProfessionalType {
    Contractor = 0,
    PlanChecker = 1,
    Bpat = 2,
    Inspector = 3,
    CsiInspector = 4,
    FogTransporter = 5,
    FogInspector = 6,
    ComponentTester = 7
}

export const professionalTypeLabels: Record<ProfessionalType, string> = {
    [ProfessionalType.Contractor]: 'Contractor',
    [ProfessionalType.PlanChecker]: 'Plan Checker',
    [ProfessionalType.Bpat]: 'BPAT',
    [ProfessionalType.Inspector]: 'Inspector',
    [ProfessionalType.CsiInspector]: 'CSI Inspector',
    [ProfessionalType.FogTransporter]: 'FOG Transporter',
    [ProfessionalType.FogInspector]: 'FOG Inspector',
    [ProfessionalType.ComponentTester]: 'Component Tester'
};
