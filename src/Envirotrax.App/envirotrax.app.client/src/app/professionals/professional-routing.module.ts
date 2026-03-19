import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WaterSuppliersComponent } from "./water-supppliers/water-suppliers.component";
import { ProfessionalUserListComponent } from "./users/list/professional-user-list.component";
import { EditProfessionalUserComponent } from "./users/edit/edit-professional-user.component";

const routes: Routes = [
    {
        path: 'water-suppliers',
        title: 'Water Supplier Registration',
        component: WaterSuppliersComponent
    },
    {
        path: 'users',
        title: 'Users',
        component: ProfessionalUserListComponent
    },
    {
        path: 'users/:id/edit',
        title: 'Edit User',
        component: EditProfessionalUserComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfessionalRoutingModule {

}
