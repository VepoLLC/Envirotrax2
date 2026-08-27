import { ExpirationType, ProfessionalType } from './professional-user-license';

export interface WaterSupplierLicense {
    id?: number;
    professionalId?: number;
    userId?: number;
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
