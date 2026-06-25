import { PhysicalType } from './fog-disposal-site-enums';

export interface FogDisposalSiteCandidate {
    id?: number;
    name?: string;
    registrationNumber?: string;
    county?: string;
    physicalType?: PhysicalType;
    isActive?: boolean;
}
