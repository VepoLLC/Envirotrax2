import { InputOption } from '@envirotrax/common-ui';
import { State } from '../lookup/state';

export enum PhysicalType {
    Other = 0,
    Landfill = 1,
    TransferStation = 2,
    ProcessingFacility = 3
}

export const physicalTypeLabels: Record<PhysicalType, string> = {
    [PhysicalType.Other]: 'Other',
    [PhysicalType.Landfill]: 'Landfill',
    [PhysicalType.TransferStation]: 'Transfer Station',
    [PhysicalType.ProcessingFacility]: 'Processing Facility'
};

export const physicalTypeOptions: InputOption[] = [
    PhysicalType.Landfill,
    PhysicalType.TransferStation,
    PhysicalType.ProcessingFacility,
    PhysicalType.Other
].map(type => ({ id: String(type), text: physicalTypeLabels[type] }));

export interface FogDisposalSite {
    id?: number;
    name?: string;
    address?: string;
    city?: string;
    state?: State | null;
    zipCode?: string;
    phoneNumber?: string;
    emailAddress?: string;
    county?: string;
    tceqRegion?: string;
    registrationNumber?: string;
    permitNumber?: string;
    physicalType?: PhysicalType;
    locationDescription?: string;
    latitude?: number | null;
    longitude?: number | null;
}

/**
 * A disposal site with its display values precomputed once after load, so the table binds plain fields.
 */
export interface FogDisposalSiteRow extends FogDisposalSite {
    cityState: string;
}
