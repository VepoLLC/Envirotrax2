export enum RenewalOptInType {
    OptedOut = 0,
    OptedIn = 1
}

export interface RenewalOptInSite {
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
    message: string;
}
