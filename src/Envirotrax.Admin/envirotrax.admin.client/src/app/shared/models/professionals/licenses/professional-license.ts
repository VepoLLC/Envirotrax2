import { ExpirationType, ProfessionalLicenseType, ProfessionalType } from './professional-user-license';

/**
 * A company-level license (ProfessionalLicenses on the server). Unlike ProfessionalUserLicense it has no user.
 * FOG transporter registrations are this kind.
 */
export interface ProfessionalLicense {
    id?: number;
    professionalType?: ProfessionalType;
    licenseType?: ProfessionalLicenseType;
    licenseNumber?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}
