import { ReferencedProfessional } from '../professionals/professional';

export enum FogVehicleCapacityType {
    Gallons = 0,
    CubicYards = 1
}

export const fogVehicleCapacityTypeLabels: Record<FogVehicleCapacityType, string> = {
    [FogVehicleCapacityType.Gallons]: 'Gallons',
    [FogVehicleCapacityType.CubicYards]: 'Cubic Yards'
};

export interface FogVehicle {
    id?: number;
    professional?: ReferencedProfessional;
    licensePlateNumber?: string;
    manufacturer?: string;
    manufacturedYear?: number;
    capacity?: number;
    capacityType?: FogVehicleCapacityType;
    stickerNumber?: string;
    createdTime?: string;
}
