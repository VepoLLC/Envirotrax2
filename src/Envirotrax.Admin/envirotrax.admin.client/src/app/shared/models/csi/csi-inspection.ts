import { PropertyType } from '../sites/site';

export enum CsiPaymentStatus {
    Paid = 1,
    Unpaid = 2
}

export class CsiInspection {
    id?: number;
    inspectionDate?: string;
    inspectionResult?: boolean;
    transactionId?: string;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: { id?: number; name?: string; code?: string };
    propertyZip?: string;
    inspectorCompanyName?: string;
    inspectorContactName?: string;
    inspectorLicenseType?: string;
    inspectorLicenseNumber?: string;
}
