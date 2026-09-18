// Mirrors Envirotrax.App.Server InsuranceValidationDto/InsuranceStatus. Deliberately not the
// ExpirationType enum: that one describes how close a date is to expiring and is shared with license
// and gauge badges, this one answers "can this professional submit work", which InsufficientCoverage
// and NotRequired have no ExpirationType equivalent for.
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
