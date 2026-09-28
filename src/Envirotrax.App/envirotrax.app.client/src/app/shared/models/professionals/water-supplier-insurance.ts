import { ProfessionalType } from './licenses/professional-user-license';

export interface WaterSupplierInsurance {
    id?: number;
    professionalId?: number;
    submittedOn?: string;
    userEmail?: string;
    companyName?: string;
    contactName?: string;
    insuranceNumber?: string;
    expirationDate?: string;
    professionalType?: ProfessionalType;
}
