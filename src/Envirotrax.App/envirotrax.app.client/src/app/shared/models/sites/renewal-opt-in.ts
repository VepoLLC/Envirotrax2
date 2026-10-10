export enum RenewalOptInType {
    OptedOut = 0,
    OptedIn = 1
}

export interface RenewalOptInSite {
    found: boolean;
    optInType: RenewalOptInType;
}

export interface RenewalOptInRequest {
    passcode: string;
    zipCodePrefix: string;
    optInType: RenewalOptInType;
    emailAddresses?: string;
}

export interface RenewalOptInTokenRequest {
    id: number;
    token: string;
}

export interface RenewalOptInResult {
    succeeded: boolean;
    message: string;
}
