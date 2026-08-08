export interface BackflowComplianceHistory {
    points: BackflowComplianceHistoryPoint[];
}

export interface BackflowComplianceHistoryPoint {
    year: number;
    month: number;
    label: string;
    total: number;
    compliant: number;
    nonCompliant: number;
    percentage: number;
}
