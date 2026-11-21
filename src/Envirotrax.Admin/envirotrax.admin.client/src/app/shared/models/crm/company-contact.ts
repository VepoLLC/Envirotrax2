import { State } from '../lookup/state';

/**
 * One of a CRM company's additional contacts (V1 Vepo Manager's Additional Contacts).
 */
export interface CompanyContact {
    id?: number;
    position?: string;
    contactName?: string;
    address?: string;
    city?: string;
    state?: State | null;
    zipCode?: string;
    phoneNumber?: string;
    cellNumber?: string;
    emailAddress?: string;

    createdTime?: string;
    updatedTime?: string;
}
