import { Professional } from "./professional";

export interface ProfessionalInsurance {
    id?: number;
    professional?: Professional;
    expirationDate?: Date;
    insuranceNumber?: string;
    filePath?: string;
}

export enum ExpirationType {
    Valid,
    AboutToExpire,
    Expired
}