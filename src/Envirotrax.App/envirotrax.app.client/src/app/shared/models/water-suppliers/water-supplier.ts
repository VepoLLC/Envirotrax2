
export class WaterSupplier {
    id?: number;
    name?: string;
    domain?: string;
    parent?: WaterSupplier;

    contactName?: string;
    pwsId?: string;
    address?: string;
    city?: string;
    state?: { id?: number; name?: string; code?: string };
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    emailAddress?: string;

    // Letter
    letterCompanyName?: string;
    letterContactName?: string;
    letterAddress?: string;
    letterCity?: string;
    letterState?: { id?: number; name?: string; code?: string };
    letterZipCode?: string;

    // Letter Contact
    letterContactCompanyName?: string;
    letterContactContactName?: string;
    letterContactAddress?: string;
    letterContactCity?: string;
    letterContactState?: { id?: number; name?: string; code?: string };
    letterContactZipCode?: string;
    letterContactPhoneNumber?: string;
    letterContactFaxNumber?: string;
    letterContactEmailAddress?: string;
}

export interface MySupplierHierarchyDto {
    adminAccount?: WaterSupplier;
    hierarchy: WaterSupplierHierarchy[];
}

export interface WaterSupplierHierarchyChild {
    waterSupplier: WaterSupplier;
    children: WaterSupplierHierarchy[];
    isExpanded?: boolean
}

export interface WaterSupplierHierarchy {
    groupLetter: string;
    waterSuppliers: WaterSupplierHierarchyChild[];
}