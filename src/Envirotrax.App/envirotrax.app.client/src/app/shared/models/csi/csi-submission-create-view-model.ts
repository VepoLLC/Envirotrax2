import { State } from '../lookup/state';
import { PropertyType } from '../../enums/property-type.enum';
import { CsiInspectionReason } from '../../enums/csi-inspection-reason.enum';

export interface CsiSubmissionCreateViewModel {
    siteId: number;
    site: CsiSiteData;
    inspector: CsiInspectorData;
    csiLicense?: CsiLicenseData;
    availableWaterSuppliers: CsiWaterSupplierOption[];
    defaultWaterSupplierId?: number;
    availableCsiAccounts: CsiAccountOption[];
    defaultCsiAccountUserId: number;
}

export interface CsiAccountOption {
    userId: number;
    contactName?: string;
    jobTitle?: string;
    emailAddress?: string;
    csiLicense?: CsiLicenseData;
}

export interface CsiSiteData {
    id: number;
    accountNumber?: string;
    businessName?: string;
    propertyType?: PropertyType;
    streetNumber?: string;
    streetName?: string;
    propertyNumber?: string;
    city?: string;
    state?: State;
    zipCode?: string;
    mailingCompanyName?: string;
    mailingContactName?: string;
    mailingStreetNumber?: string;
    mailingStreetName?: string;
    mailingNumber?: string;
    mailingCity?: string;
    mailingState?: State;
    mailingZipCode?: string;
    mailingPhoneNumber?: string;
    mailingEmailAddress?: string;
}

export interface CsiInspectorData {
    companyName?: string;
    contactName?: string;
    jobTitle?: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    phoneNumber?: string;
    faxNumber?: string;
    emailAddress?: string;
}

export interface CsiLicenseData {
    licenseNumber?: string;
    licenseTypeName?: string;
    expirationDate?: string;
    isValid: boolean;
}

export interface CsiWaterSupplierOption {
    id: number;
    name: string;
    pwsId?: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    phoneNumber?: string;
    contactName?: string;
    emailAddress?: string;
}

export interface CsiSubmissionSaveRequest {
    siteId: number;
    waterSupplierId: number;
    selectedCsiAccountUserId: number;
    inspectionDate?: string;
    reasonForInspection: CsiInspectionReason;
    compliance1: boolean;
    compliance2: boolean;
    compliance3: boolean;
    compliance4: boolean;
    compliance5: boolean;
    compliance6: boolean;
    materialServiceLineLead: boolean;
    materialServiceLineCopper: boolean;
    materialServiceLinePVC: boolean;
    materialServiceLineOther: boolean;
    materialServiceLineOtherDescription?: string;
    materialSolderLead: boolean;
    materialSolderLeadFree: boolean;
    materialSolderSolventWeld: boolean;
    materialSolderOther: boolean;
    materialSolderOtherDescription?: string;
    comments?: string;
}
