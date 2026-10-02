export enum InsuranceCheckResult {
    NotRequired,
    NotFound,
    Unverified,
    Expired,
    InvalidCoverage,
    Valid
}

export interface InsuranceCheck {
    result: InsuranceCheckResult;
    requiredAmount: number;
    message?: string;
    isSatisfied: boolean;
}
