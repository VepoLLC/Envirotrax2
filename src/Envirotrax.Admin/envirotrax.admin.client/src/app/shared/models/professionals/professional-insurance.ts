import { ReferencedProfessional } from './professional';
import { ExpirationType } from './licenses/professional-user-license';

export interface ProfessionalInsurance {
    id?: number;
    professional?: ReferencedProfessional;
    expirationDate?: string;
    insuranceNumber?: string;
    coverageAmount?: number;
    filePath?: string;
    expirationType?: ExpirationType;
}
