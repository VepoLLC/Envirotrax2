import { PropertyType } from "../../enums/property-type.enum";

export interface BackflowRenewalRequirement {
    id?: number;
    propertyType?: PropertyType;
    deviceType?: string;
    hazardType?: string;
    hasSiteOssf?: boolean;
    auxWaterSupply?: boolean;
    renewalYears?: number;
}
