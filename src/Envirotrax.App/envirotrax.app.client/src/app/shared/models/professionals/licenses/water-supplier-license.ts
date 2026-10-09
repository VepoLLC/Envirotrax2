import { ExpirationType, ProfessionalType } from "./professional-user-license";
import { LicenseScope } from "./professional-license-type";

export interface WaterSupplierLicense {
    id?: number;
    licenseScope?: LicenseScope;
    professionalId?: number;
    userId?: number;
    submittedOn?: string;
    userEmail?: string;
    companyName?: string;
    contactName?: string;
    professionalType?: ProfessionalType;
    licenseTypeId?: number;
    licenseTypeName?: string;
    licenseNumber?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}

export interface UpdateWaterSupplierLicense {
    licenseNumber: string;
    contactName?: string;
    expirationDate?: string;
}

export interface LicenseCounts {
    unverifiedCount: number;
    expiredCount: number;
    expiringCount: number;
}
