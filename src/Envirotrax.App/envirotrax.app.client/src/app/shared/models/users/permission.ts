import { PermissionType } from "../permission-type";


export interface Permission {
    id?: PermissionType,
    name?: string;
    category?: string;

    canView?: boolean;
    canModify?: boolean;
    canDelete?: boolean;
}