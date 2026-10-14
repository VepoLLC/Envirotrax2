import { ExpirationType, ProfessionalType } from "./licenses/professional-user-license";

export interface ProfessionalDashboardLicenseInsurance {
    id?: number;
    rowType?: ProfessionalDashboardRowType;
    professionalType?: ProfessionalType;
    typeName?: string;
    number?: string;
    assignedTo?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}

export enum ProfessionalDashboardRowType {
    License,
    Insurance,
    CompanyLicense
}

export { ExpirationType };
