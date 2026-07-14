export interface BackflowNewRemovedReport {
    points: BackflowNewRemovedPoint[];
}

export interface BackflowNewRemovedPoint {
    year: number;
    month: number;
    label: string;
    created: number;
    removed: number;
}
