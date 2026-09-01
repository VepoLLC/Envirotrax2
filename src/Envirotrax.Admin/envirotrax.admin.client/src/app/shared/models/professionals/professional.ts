import { State } from '../lookup/state';

export interface Professional {
    id?: number;
    parentId?: number;
    name?: string;
    companyEmail?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    webSiteUrl?: string;
    hidePublicListing?: boolean;
    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;
    createdTime?: string;
}

export interface ReferencedProfessional {
    id?: number;
    name?: string;
}

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
}
