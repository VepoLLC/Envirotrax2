export class WaterSupplierUser {
    id?: number;
    contactName?: string;
    emailAddress?: string;
    cellNumber?: string;
    roles?: Role[];
}

export class Role {
    name?: string;
}
