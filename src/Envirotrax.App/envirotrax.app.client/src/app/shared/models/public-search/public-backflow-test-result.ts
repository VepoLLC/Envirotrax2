export interface PublicBackflowTestResult {
    id: number;
    testDate?: string;
    expirationDate?: string;
    renewalRequired: boolean;
    manufacturer?: string;
    model?: string;
    size?: string;
    deviceType?: string;
    serialNumber?: string;
    hazardType?: string;
    hazardTypeOtherDescription?: string;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: string;
    propertyZip?: string;
}
