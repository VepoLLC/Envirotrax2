import { Professional } from '../professionals/professional';
import { FogVehicleCapacityType } from './fog-vehicle-enums';

export interface FogVehicle {
    id?: number;
    professional?: Professional | null;
    licensePlateNumber?: string;
    manufacturer?: string;
    manufacturedYear?: number;
    capacity?: number;
    capacityType?: FogVehicleCapacityType;
    stickerNumber?: string;
    createdTime?: string;
}
