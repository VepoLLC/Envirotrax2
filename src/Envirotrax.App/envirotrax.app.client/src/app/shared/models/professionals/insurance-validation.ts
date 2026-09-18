export enum InsuranceStatus {
    NotFound = 0,
    AwaitingValidation = 1,
    Expired = 2,
    InsufficientCoverage = 3,
    Valid = 4,
    NotRequired = 5
}

export interface InsuranceValidation {
    status: InsuranceStatus;
    requiredCoverage: number;
}
