import { State } from '../lookup/state';
import { PhysicalType } from './fog-disposal-site-enums';

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
    latitude?: number;
    longitude?: number;
}
