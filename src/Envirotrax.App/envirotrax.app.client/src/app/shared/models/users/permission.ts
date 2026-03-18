import { PermissionType } from "../permission-type";


export interface Permission {
    id?: PermissionType,
    name?: string;
    category?: string;

    canView?: boolean;
    canCreate?: boolean;
    canEdit?: boolean;
    canDelete?: boolean;
}