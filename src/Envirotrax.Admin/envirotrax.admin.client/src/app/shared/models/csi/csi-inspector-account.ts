import { State } from '../lookup/state';
import { Professional, ProfessionalUser } from '../professionals/professional';
import { ProfessionalWaterSupplier } from '../professionals/professional-water-supplier';

export class CsiInspectorAccount {
    id?: number;
    professionalId?: number;
    emailAddress?: string;
    companyName?: string;
    contactName?: string;
    jobTitle?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    workNumber?: string;
}

export interface CsiInspectorAccountDetails {
    professional: Professional;
    user?: ProfessionalUser;
    registrations: ProfessionalWaterSupplier[];
}
