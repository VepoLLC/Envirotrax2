export enum PhysicalType {
    Other = 0,
    Landfill = 1,
    TransferStation = 2,
    ProcessingFacility = 3
}

export const PHYSICAL_TYPE_LABELS: Record<PhysicalType, string> = {
    [PhysicalType.Other]: 'Other',
    [PhysicalType.Landfill]: 'Landfill',
    [PhysicalType.TransferStation]: 'Transfer Station',
    [PhysicalType.ProcessingFacility]: 'Processing Facility'
};
