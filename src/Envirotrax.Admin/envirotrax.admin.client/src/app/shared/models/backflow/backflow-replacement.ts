import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';
import { WaterSupplier } from '../water-suppliers/water-supplier';
import { BackflowTestSite } from './backflow-test';

export class BackflowReplacement {
    id?: number;
    waterSupplier?: WaterSupplier;
    site?: BackflowTestSite;
    validationReplacementOnHold?: boolean;
    validationReplacementCleared?: boolean;
    replacementAssembly?: string;
    deviceType?: string;
    manufacturer?: string;
    model?: string;
    size?: string;
    serialNumber?: string;
    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;
}
