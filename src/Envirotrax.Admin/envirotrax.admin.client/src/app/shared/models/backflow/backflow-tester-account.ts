import { State } from '../lookup/state';

export class BackflowTesterAccount {
    id?: number;
    professionalId?: number;
    emailAddress?: string;
    companyName?: string;
    contactName?: string;
    jobTitle?: string;
    address?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    workNumber?: string;
    cellNumber?: string;
    isSubAccount?: boolean;
}
