import { State } from "../lookup/state";
import { ExpirationType, ProfessionalUser } from "./professional-user";

export interface Professional {
    id?: number;
    name?: string;
    address?: string
    city?: string;
    state?: State;
    zipCode?: string;
    companyEmail?: string;
    phoneNumber?: string;
    faxNumber?: string;
    websiteUrl?: string;
    hidePublicListing?: boolean;
    createdTime?: string;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;

    insuranceExpirationType?: ExpirationType;
}

export interface CreateProfessional {
    professional: Professional;
    user: ProfessionalUser
}