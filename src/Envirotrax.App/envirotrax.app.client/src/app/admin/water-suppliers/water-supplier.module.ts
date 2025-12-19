import { NgModule } from "@angular/core";
import { SupplierListComponent } from "./list/supplier-list.component";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { WaterSupplierRoutingModule } from "./water-supplier-routing.module";

@NgModule({
    declarations: [
        SupplierListComponent
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