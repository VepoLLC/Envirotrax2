export enum LicenseCheckResult {
    NotRequired,
    NotFound,
    Unverified,
    Expired,
    Valid
}

export interface LicenseCheck {
    result: LicenseCheckResult;
    label?: string;
    message?: string;
    licenseNumber?: string;
    licenseTypeName?: string;
    expirationDate?: string;
    isSatisfied: boolean;
}

export interface BpatLicenseCheck {
    license: LicenseCheck;
    fireLicense: LicenseCheck;
}
