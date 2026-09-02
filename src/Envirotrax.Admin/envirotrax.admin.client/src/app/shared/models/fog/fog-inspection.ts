import { State } from '../lookup/state';
import { PropertyType } from '../sites/site';

export enum FogInspectionResult {
    Passed = 0,
    Failed = 1
}

export enum FogPaymentStatus {
    Paid = 1,
    Unpaid = 2
}

/**
 * V1 offers the total capacity criteria as two fixed buckets around 25% rather than
 * a free numeric range, so it is sent to the API as its own parameter.
 */
export enum FogTotalCapacityRange {
    TwentyFivePercentOrLess = 1,
    GreaterThanTwentyFivePercent = 2
}

export enum InterceptorType {
    GreaseTrap = 'Grease Trap',
    GritTrap = 'Grit Trap',
    SepticTank = 'Septic Tank',
    ChemicalToilet = 'Chemical Toilet',
    Other = 'Other'
}

export enum InterceptorCapacityType {
    Gallons = 0,
    CubicYards = 1
}

export const interceptorCapacityTypeLabels: Record<InterceptorCapacityType, string> = {
    [InterceptorCapacityType.Gallons]: 'Gallons',
    [InterceptorCapacityType.CubicYards]: 'Cubic Yards'
};

export class FogInspection {
    id?: number;
    inspectionDate?: string;
    inspectionResult?: FogInspectionResult;
    transactionId?: string;
    totalCapacityPercent?: number;

    propertyType?: PropertyType;
    propertyBusinessName?: string;
    propertyStreetNumber?: string;
    propertyStreetName?: string;
    propertyNumber?: string;
    propertyCity?: string;
    propertyState?: State;
    propertyZip?: string;

    interceptorType?: string;
    interceptorOtherDescription?: string;
    interceptorCapacity?: number;
    interceptorCapacityType?: InterceptorCapacityType;
    interceptorLocationDescription?: string;

    inspectorCompanyName?: string;
    inspectorContactName?: string;

    createdTime?: string;
}

/**
 * Search result row: display strings are pre-computed once after each load so the
 * templates only interpolate plain fields.
 */
export interface FogInspectionRow extends FogInspection {
    propertyAddress: string;
    propertyCityStateZip: string;
    interceptorDescription: string;
}
