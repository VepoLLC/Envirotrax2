import { PermissionType } from '../permission-type';

export class WaterSupplierUserAccount {
    id?: number;
    contactName?: string;
    emailAddress?: string;
    cellNumber?: string;
    permissions?: UserAccountPermission[];
}

export class UserAccountPermission {
    permission?: PermissionType;
    canView?: boolean;
    canModify?: boolean;
}
