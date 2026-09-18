import { Professional } from "./professional";
import { ExpirationType } from "./licenses/professional-user-license";

export interface ProfessionalInsurance {
    id?: number;
    professional?: Professional;
    expirationDate?: Date;
    insuranceCoverage?: number;
    insuranceNumber?: string;
    filePath?: string;
    expirationType?: ExpirationType;
}

export { ExpirationType };
