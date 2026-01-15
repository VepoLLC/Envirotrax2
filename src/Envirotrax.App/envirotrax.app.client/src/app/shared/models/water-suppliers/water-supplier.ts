
export class WaterSupplier {
    id?: number;
    name?: string;
    domain?: string;
    parent?: WaterSupplier;

    contactName?: string;
    pwsId?: string;
    address?: string;
    city?: string;
    stateId?: number | null;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    emailAddress?: string;

    // Letter
    letterCompanyName?: string;
    letterContactName?: string;
    letterAddress?: string;
    letterCity?: string;
    letterStateId?: number | null;
    letterZipCode?: string;

    // Letter Contact
    letterContactCompanyName?: string;
    letterContactContactName?: string;
    letterContactAddress?: string;
    letterContactCity?: string;
    letterContactStateId?: number | null;
    letterContactZipCode?: string;
    letterContactPhoneNumber?: string;
    letterContactFaxNumber?: string;
    letterContactEmailAddress?: string;
}
