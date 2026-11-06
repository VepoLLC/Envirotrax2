import { ExpirationType, LicenseScope, ProfessionalLicenseType, ProfessionalType } from './professional-user-license';

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
    lastCheckDate?: string;
    hoursSinceLastCheck?: number;
}

export interface ProfessionalLicense {
    id?: number;
    professionalType?: ProfessionalType;
    licenseType?: ProfessionalLicenseType;
    licenseNumber?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}

export interface LicenseAdminCounts {
    unverifiedCount: number;
    expiredCount: number;
    expiringCount: number;
    insuranceCount: number;
    gaugeCount: number;
}

export type LicenseFilter = 'unverified' | 'expired' | 'expiring';
