export interface FogInspectionDailyStats {
    date: string;
    isWeekend: boolean;
    totalInspections: number;
    totalPaidInspections: number;
}

export interface FogInspectionSubAccountStats {
    waterSupplierId: number;
    waterSupplierName: string;
    dailyStats: FogInspectionDailyStats[];
}

export interface FogInspectionSubmissionStats {
    dailyStats: FogInspectionDailyStats[];
    subAccountStats?: FogInspectionSubAccountStats[] | null;
}
