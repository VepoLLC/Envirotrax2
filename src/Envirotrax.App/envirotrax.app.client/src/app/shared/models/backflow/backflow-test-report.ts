export interface BackflowTestReport {
    totalCount: number;
    periods: BackflowReportPeriod[];
    subAccounts: BackflowSubAccountReportItem[];
    stats: BackflowReportStatCategory[];
}

export interface BackflowSubAccountReportItem {
    name: string;
    count: number;
    percentage: number;
}

export interface BackflowReportPeriod {
    label: string;
    count: number;
    percentage: number;
    year?: number | null;
    month?: number | null;
}

export interface BackflowReportStatCategory {
    title: string;
    items: BackflowReportStatItem[];
}

export interface BackflowReportStatItem {
    label: string;
    count: number;
    percentage: number;
}
