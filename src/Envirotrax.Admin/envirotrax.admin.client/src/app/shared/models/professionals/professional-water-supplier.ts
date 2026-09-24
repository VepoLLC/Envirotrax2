import { ReferencedProfessional } from './professional';
import { WaterSupplier } from '../water-suppliers/water-supplier';

export interface ProfessionalWaterSupplier {
    waterSupplier?: WaterSupplier;
    professional?: ReferencedProfessional;

    hasWiseGuys?: boolean;
    hasBackflowTesting?: boolean;
    hasCsiInspection?: boolean;
    hasFogInspection?: boolean;
    hasFogTransportation?: boolean;

    isBackflowTestingSuspended?: boolean;
    isCsiInspectionSuspended?: boolean;
    isFogInspectionSuspended?: boolean;
    isFogTransportationSuspended?: boolean;

    backflowResidentialTestFee?: number | null;
    backflowCommercialTestFee?: number | null;
    csiCommercialInspectionFee?: number | null;
    csiResidentialInspectionFee?: number | null;
    fogTransportFee?: number | null;
    fogInspectorFee?: number | null;
}
