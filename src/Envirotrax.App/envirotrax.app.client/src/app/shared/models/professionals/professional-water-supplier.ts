import { WaterSupplier } from "../water-suppliers/water-supplier";

export interface ProfessionalWaterSupplier {
    waterSupplier?: WaterSupplier;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;

    isBanned?: boolean;

    residentialFee?: number;
    commercialFee?: number;
    fogFee?: boolean;
}