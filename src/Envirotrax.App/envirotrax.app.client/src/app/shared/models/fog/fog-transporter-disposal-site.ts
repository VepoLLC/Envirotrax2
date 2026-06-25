import { WaterSupplier } from '../water-suppliers/water-supplier';
import { Professional } from '../professionals/professional';

export interface FogTransporterDisposalSite {
    id?: number;
    waterSupplierId?: number;
    waterSupplier?: WaterSupplier | null;
    professional?: Professional | null;
    isActive?: boolean;
}
