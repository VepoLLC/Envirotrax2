export interface CsiSystemReport {
    totalCount: number;
    periods: CsiReportPeriod[];
    subAccounts: CsiSubAccountReportItem[];
    stats: CsiReportStatCategory[];
}

export interface CsiSubAccountReportItem {
    name: string;
    count: number;
    percentage: number;
}

export interface CsiReportPeriod {
    label: string;
    count: number;
    percentage: number;
    year?: number | null;
    month?: number | null;
}

export interface CsiReportStatCategory {
    title: string;
    items: CsiReportStatItem[];
}

export interface CsiReportStatItem {
    label: string;
    count: number;
    percentage: number;
}
