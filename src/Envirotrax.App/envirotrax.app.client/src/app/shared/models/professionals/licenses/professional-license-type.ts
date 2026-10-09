import { State } from "../../lookup/state";
import { ProfessionalType } from "./professional-user-license";

export interface ProfessionalLicenseType {
    id?: number;
    name?: string;
    description?: string;
    professionalType?: ProfessionalType;
    licenseScope?: LicenseScope;
    state?: State;
}

export enum LicenseScope {
    User,
    Company
}
