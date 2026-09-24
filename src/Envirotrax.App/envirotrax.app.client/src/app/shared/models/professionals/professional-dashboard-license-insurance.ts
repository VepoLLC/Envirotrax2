import { ExpirationType } from "./licenses/professional-user-license";

export interface ProfessionalDashboardLicenseInsurance {
    id?: number;
    rowType?: ProfessionalDashboardRowType;
    typeName?: string;
    number?: string;
    assignedTo?: string;
    expirationDate?: string;
    expirationType?: ExpirationType;
}

export enum ProfessionalDashboardRowType {
    License,
    Insurance
}

export { ExpirationType };
