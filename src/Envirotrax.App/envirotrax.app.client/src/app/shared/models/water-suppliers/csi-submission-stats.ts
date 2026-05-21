export interface CsiDailyStats {
    date: string;
    totalInspections: number;
    totalPaidInspections: number;
}

export interface CsiSubAccountStats {
    waterSupplierName: string;
    dailyStats: CsiDailyStats[];
}

export interface CsiSubmissionStats {
    dailyStats: CsiDailyStats[];
    subAccountStats?: CsiSubAccountStats[] | null;
}
