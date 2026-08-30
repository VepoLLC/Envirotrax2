import { Professional } from '../professionals/professional';
import { FogVehicleCapacityType, FogVehicleInspectionDueStatus } from './fog-vehicle-enums';

// A water supplier's permit for a vehicle. Its identity is the vehicle it covers.
export interface FogVehiclePermit {
    id?: number;
    permitNumber?: string;
    inspectionDueDate?: string | null;
    isActive?: boolean;
    createdTime?: string;
}

// A row of the Vehicle Permit Management search: a vehicle of a registered transporter, with permit
// left null when this water supplier has not issued one for it yet.
export interface FogVehiclePermitSearch {
    id?: number;

    professional?: Professional | null;

    licensePlateNumber?: string;
    manufacturer?: string;
    manufacturedYear?: number;
    capacity?: number;
    capacityType?: FogVehicleCapacityType;
    stickerNumber?: string;

    permit?: FogVehiclePermit | null;

    inspectionDueStatus?: FogVehicleInspectionDueStatus;
}
