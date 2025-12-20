import { NgModule } from "@angular/core";
import { SupplierListComponent } from "./list/supplier-list.component";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { WaterSupplierRoutingModule } from "./water-supplier-routing.module";
import { CreateSupplierComponent } from "./create/create-supplier.component";
import { EditSupplierComponent } from "./edit/edit-supplier.component";

@NgModule({
    declarations: [
        SupplierListComponent,
        CreateSupplierComponent,
        EditSupplierComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        WaterSupplierRoutingModule
    ]
})
export class WaterSupplierModule {

}