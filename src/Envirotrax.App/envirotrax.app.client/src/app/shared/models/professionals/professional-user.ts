import { State } from "../lookup/state";

export interface ProfessionalUser {
    id?: number;
    emailAddress?: string;
    contactName?: string;
    jobTitle?: string;
    signaturePath?: string;
    signatureUrl?: string;
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

    billingFirstName?: string;
    billingLastName?: string;
    billingAddress?: string;
    billingCity?: string;
    billingState?: State;
    billingZipCode?: string;
}

export enum ExpirationType {
    Valid = 0,
    AboutToExpire = 1,
    Expired = 2
}