import { ProfessionalLicenseType } from "./professional-license-type";
import { ExpirationType, ProfessionalType } from "./professional-user-license";

export interface ProfessionalLicense {
    id?: number;
    professionalType?: ProfessionalType;
    licenseType?: ProfessionalLicenseType;
    licenseNumber?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}
