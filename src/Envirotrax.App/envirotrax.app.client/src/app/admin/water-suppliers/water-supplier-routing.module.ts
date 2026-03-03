import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SupplierListComponent } from "./list/supplier-list.component";
import { EditSupplierComponent } from "./edit/edit-supplier.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";

const routes: Routes = [
    {
        path: '',
        title: 'Water Suppliers',
        component: SupplierListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.WaterSuppliers,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
    {
        path: ':id/edit',
        title: 'Edit Water Supplier',
        component: EditSupplierComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.WaterSuppliers,
                    action: PermissionAction.CanEdit
                }
            ]
        }
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class WaterSupplierRoutingModule {

}