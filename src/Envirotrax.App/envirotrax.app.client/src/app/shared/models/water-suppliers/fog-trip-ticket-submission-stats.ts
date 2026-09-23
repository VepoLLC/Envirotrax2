export interface FogTripTicketDailyStats {
    date: string;
    isWeekend: boolean;
    totalTripTickets: number;
    totalPaidTripTickets: number;
}

export interface FogTripTicketSubAccountStats {
    waterSupplierId: number;
    waterSupplierName: string;
    dailyStats: FogTripTicketDailyStats[];
}

export interface FogTripTicketSubmissionStats {
    dailyStats: FogTripTicketDailyStats[];
    subAccountStats?: FogTripTicketSubAccountStats[] | null;
}
