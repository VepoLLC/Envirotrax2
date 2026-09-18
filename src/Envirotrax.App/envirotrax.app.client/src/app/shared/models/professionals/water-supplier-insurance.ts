import { ExpirationType, ProfessionalType } from './licenses/professional-user-license';

export interface WaterSupplierInsurance {
    id?: number;
    professionalId?: number;
    companyName?: string;
    companyEmail?: string;
    insuranceNumber?: string;
    insuranceCoverage?: number;
    expirationDate?: string;
    expirationType?: ExpirationType;
    professionalType?: ProfessionalType;
}

export interface UpdateWaterSupplierInsurance {
    insuranceNumber: string;
    expirationDate?: string;
    insuranceCoverage?: number;
}

export interface InsuranceCounts {
    unverifiedCount: number;
    expiredCount: number;
    expiringCount: number;
}
