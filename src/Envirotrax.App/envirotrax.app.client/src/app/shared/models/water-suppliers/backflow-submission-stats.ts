export interface BackflowDailyStats {
    date: string;
    isWeekend: boolean;
    totalTests: number;
    totalPaidTests: number;
}

export interface BackflowSubAccountStats {
    waterSupplierName: string;
    dailyStats: BackflowDailyStats[];
}

export interface BackflowSubmissionStats {
    dailyStats: BackflowDailyStats[];
    subAccountStats?: BackflowSubAccountStats[] | null;
}
