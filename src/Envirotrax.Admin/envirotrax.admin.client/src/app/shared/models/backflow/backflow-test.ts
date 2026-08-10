import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { WaterSupplier } from '../water-suppliers/water-supplier';

export enum BackflowPaymentStatus {
    Paid = 1,
    Unpaid = 2
}

export enum BackflowTestResult {
    Pass = 0,
    Fail = 1,
    PassAfterRepairs = 2
}

export enum BackflowReasonForTest {
    AnnualTest = 0,
    NewInstallation = 1,
    ExistingInstallation = 2,
    Replacement = 3,
    Repair = 4,
    AnnualTestAfterRepairs = 5
}

export class BackflowTest {
    id?: number;
    waterSupplier?: WaterSupplier;
    accountNumber?: string;
    createdTime?: string;
    testDate?: string;
    expirationDate?: string;
    testResult?: BackflowTestResult;
    isCurrent?: boolean;
    outOfService?: boolean;
    disapproved?: boolean;
    rejected?: boolean;
    transactionId?: string;
    serialNumber?: string;
    serialNumber2?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
    hazardType?: string;
    locationDescription?: string;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;
    bpatCompanyName?: string;
    bpatContactName?: string;
    bpatLicenseNumber?: string;
}
