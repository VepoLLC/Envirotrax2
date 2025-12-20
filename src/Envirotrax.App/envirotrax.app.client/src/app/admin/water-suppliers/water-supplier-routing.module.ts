import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { SupplierListComponent } from "./list/supplier-list.component";

const routes: Routes = [
    {
        path: '',
        title: 'Water Suppliers',
        component: SupplierListComponent
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