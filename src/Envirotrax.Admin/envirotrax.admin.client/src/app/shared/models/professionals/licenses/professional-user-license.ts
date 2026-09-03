import { ProfessionalUser } from '../professional';
import { State } from '../../lookup/state';

export interface ProfessionalUserLicense {
    id?: number;
    user?: ProfessionalUser;
    professionalType?: ProfessionalType;
    licenseType?: ProfessionalLicenseType;
    licenseNumber?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}

export interface ProfessionalLicenseType {
    id?: number;
    name?: string;
    description?: string;
    professionalType?: ProfessionalType;
    state?: State;
}

export enum ExpirationType {
    Valid = 0,
    AboutToExpire = 1,
    Expired = 2
}

export const expirationTypeCssClasses: Record<ExpirationType, string> = {
    [ExpirationType.Valid]: 'text-bg-success',
    [ExpirationType.AboutToExpire]: 'text-bg-warning',
    [ExpirationType.Expired]: 'text-bg-danger'
};

// Shown while the record has no expiration date yet, so it is not one of the enum states.
export const unvalidatedExpirationCssClass = 'text-bg-primary';

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
