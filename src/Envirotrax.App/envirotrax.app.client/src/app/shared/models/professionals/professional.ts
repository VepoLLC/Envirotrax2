import { State } from "../states/state";

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
}