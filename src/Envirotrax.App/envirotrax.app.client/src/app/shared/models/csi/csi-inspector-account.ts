import { State } from '../lookup/state';

export interface CsiInspectorAccount {
    id?: number;
    name?: string;
    companyEmail?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    createdTime?: string;
}
