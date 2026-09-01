export interface CsiDailyStats {
    date: string;
    isWeekend: boolean;
    totalInspections: number;
    totalPaidInspections: number;
}

export interface CsiSubAccountStats {
    waterSupplierId: number;
    waterSupplierName: string;
    dailyStats: CsiDailyStats[];
}

export interface CsiSubmissionStats {
    dailyStats: CsiDailyStats[];
    subAccountStats?: CsiSubAccountStats[] | null;
}
