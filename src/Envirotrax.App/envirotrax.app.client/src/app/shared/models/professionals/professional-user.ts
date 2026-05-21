
export interface ProfessionalUser {
    id?: number;
    emailAddress?: string;
    contactName?: string;
    jobTitle?: string;
    isAdmin?: boolean;
    isWiseGuy?: boolean;
    isCsiInspector?: boolean;
    isBackflowTester?: boolean;
    isFogInspector?: boolean;
    isFogTransporter?: boolean;

    bpatLicenseNumber?: string;
    bpatLicenseTypeName?: string;
    bpatLicenseExpirationDate?: string;
    bpatLicenseExpirationType?: ExpirationType;
}

export enum ExpirationType {
    Valid = 0,
    AboutToExpire = 1,
    Expired = 2
}