import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SupplierListComponent } from "./list/supplier-list.component";
import { EditSupplierComponent } from "./edit/edit-supplier.component";

const routes: Routes = [
    {
        path: '',
        title: 'Water Suppliers',
        component: SupplierListComponent
    },
    {
        path: ':id/edit',
        title: 'Edit Water Supplier',
        component: EditSupplierComponent
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