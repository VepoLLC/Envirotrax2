import { Permission } from "./permission";
import { Role } from "./role";

export interface RolePermission {
    role?: Role;
    permission?: Permission;

    canView?: boolean;
    canModify?: boolean;
    canDelete?: boolean;
}