export interface PublicSearchWaterSupplier {
    id: number;
    name: string;
}

export interface PublicSearchWaterSuppliers {
    suppliers: PublicSearchWaterSupplier[];
    selectedWaterSupplierId?: number | null;
}
