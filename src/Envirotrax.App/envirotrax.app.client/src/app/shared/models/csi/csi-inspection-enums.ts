export interface CsiProfessionalSearchRequest {
    latestOnly: boolean;
    passFail: string;
    dateType: string;
    fromDate?: string | null;
    toDate?: string | null;
}
