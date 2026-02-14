import { WaterSupplier } from "../water-suppliers/water-supplier";

export interface ProfessionalWaterSupplier {
    waterSupplier?: WaterSupplier;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;

    isBanned?: boolean;

    backflowResidentialTestFee?: number;
    backflowCommercialTestFee?: number;
    csiCommercialInspectionFee?: number;
    csiResidentialInspectionFee?: number;
    fogTransportFee?: number;
}

export interface AvailableWaterSupplier {
    id?: number;
    name?: string;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;

    backflowResidentialTestFee?: number;
    backflowCommercialTestFee?: number;
    csiCommercialInspectionFee?: number;
    csiResidentialInspectionFee?: number;
    fogTransportFee?: number;
}