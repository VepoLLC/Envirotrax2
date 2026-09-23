export enum FogVehicleCapacityType {
    Gallons = 0,
    CubicYards = 1
}

export const FOG_VEHICLE_CAPACITY_TYPE_LABELS: Record<FogVehicleCapacityType, string> = {
    [FogVehicleCapacityType.Gallons]: 'Gallons',
    [FogVehicleCapacityType.CubicYards]: 'Cubic Yards'
};

export enum FogVehicleInspectionDueStatus {
    None = 0,
    Current = 1,
    PastDue = 2
}
