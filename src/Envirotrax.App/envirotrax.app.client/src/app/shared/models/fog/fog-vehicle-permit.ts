import { FogVehicleCapacityType, FogVehicleInspectionDueStatus } from './fog-vehicle-enums';

// A water supplier's permit for a vehicle. Its identity within a supplier is the vehicle it covers.
export interface FogVehiclePermit {
    id?: number;
    permitNumber?: string;
    inspectionDueDate?: string | null;
    isActive?: boolean;
    createdTime?: string;
    updatedTime?: string;
}

// A row of the Vehicle Permit Management search: one per vehicle in the water supplier's scope, with
// the permit fields left null when no permit has been issued for it yet.
export interface FogVehiclePermitSearch {
    id?: number;

    transporterId?: number;
    transporterCompanyName?: string;
    transporterAddress?: string;
    transporterCity?: string;
    transporterState?: string;
    transporterZip?: string;
    transporterPhoneNumber?: string;
    transporterFaxNumber?: string;
    transporterEmailAddress?: string;

    licensePlateNumber?: string;
    manufacturer?: string;
    manufacturedYear?: number;
    capacity?: number;
    capacityType?: FogVehicleCapacityType;
    stickerNumber?: string;

    hasPermit?: boolean;
    permitNumber?: string | null;
    inspectionDueDate?: string | null;
    isActive?: boolean | null;

    inspectionDueStatus?: FogVehicleInspectionDueStatus;
}
