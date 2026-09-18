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
    // Not stored on the policy - resolved server-side from the professional's registered programs,
    // purely so the Manage button knows which per-type details page to open.
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
