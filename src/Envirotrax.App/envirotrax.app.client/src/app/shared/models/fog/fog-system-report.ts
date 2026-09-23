export enum FogTripTicketReportDateType {
    GeneratorRemovalDate = 0,
    ReceiverDeliveryDate = 1
}

export interface FogSystemReport {
    totalCount: number;
    totalGallons: number;
    totalCubicFeet: number;
    periods: FogReportPeriod[];
    subAccounts: FogSubAccountReportItem[];
    stats: FogReportStatCategory[];
}

export interface FogReportPeriod {
    label: string;
    count: number;
    percentage: number;
    gallons: number;
    cubicFeet: number;
    year?: number | null;
    month?: number | null;
}

export interface FogSubAccountReportItem {
    name: string;
    count: number;
    percentage: number;
    gallons: number;
    cubicFeet: number;
}

export interface FogReportStatCategory {
    title: string;
    items: FogReportStatItem[];
}

export interface FogReportStatItem {
    label: string;
    count: number;
    percentage: number;
    gallons: number;
    cubicFeet: number;
}
