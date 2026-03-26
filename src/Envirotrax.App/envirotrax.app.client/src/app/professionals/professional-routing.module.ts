import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WaterSuppliersComponent } from "./water-supppliers/water-suppliers.component";
import { ProfessionalUserListComponent } from "./users/list/professional-user-list.component";
import { EditProfessionalUserComponent } from "./users/edit/edit-professional-user.component";
import { SiteListComponent } from "./sites/site-list.component";
import { RoleGuard } from "../shared/guards/role.guard";
import { ROLE_DEFINITIONS } from "../shared/models/role-definitions";

const routes: Routes = [
    {
        path: 'water-suppliers',
        title: 'Water Supplier Registration',
        component: WaterSuppliersComponent,
        canActivate: [RoleGuard],
        data: {
            roles: [ROLE_DEFINITIONS.PROFESSIONALS.ADMIN]
        }
    },
    {
        path: 'users',
        title: 'Users',
        component: ProfessionalUserListComponent,
        canActivate: [RoleGuard],
        data: {
            roles: [ROLE_DEFINITIONS.PROFESSIONALS.ADMIN]
        }
    },
    {
        path: 'users/:id/edit',
        title: 'Edit User',
        component: EditProfessionalUserComponent,
        canActivate: [RoleGuard],
        data: {
            roles: [ROLE_DEFINITIONS.PROFESSIONALS.ADMIN]
        }
    },
    {
        path: 'sites',
        title: 'Property Records',
        component: SiteListComponent,
        canActivate: [RoleGuard],
        data: {
            roles: [ROLE_DEFINITIONS.PROFESSIONAL]
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalRoutingModule {

}
