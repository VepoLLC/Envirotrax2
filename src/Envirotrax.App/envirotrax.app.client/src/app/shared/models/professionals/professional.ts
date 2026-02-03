import { State } from "../states/state";
import { ProfessionalUser } from "./professional-user";

export interface Professional {
    id?: number;
    name?: string;
    address?: string
    city?: string;
    state?: State;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    websiteUrl?: string;
    hidePublicListing?: boolean;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;
}

export interface CreateProfessional {
    professional: Professional;
    user: ProfessionalUser
}