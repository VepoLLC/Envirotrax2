import { State } from '../lookup/state';

export class CsiInspectorAccount {
    id?: number;
    professionalId?: number;
    isMasterAccount?: boolean;
    emailAddress?: string;
    companyName?: string;
    contactName?: string;
    jobTitle?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    workNumber?: string;
}
