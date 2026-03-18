import { Role } from "./role";
import { WaterSupplierUser } from "./water-supplier-user";

export interface UserRole {
    user?: WaterSupplierUser;
    role?: Role;
}