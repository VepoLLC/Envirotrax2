import { NgModule } from "@angular/core";
import { ProfessionalRoutingModule } from "./professional-routing.module";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { WaterSuppliersComponent } from "./water-supppliers/water-suppliers.component";

@NgModule({
    declarations: [
        WaterSuppliersComponent
    ],
    imports: [
        ProfessionalRoutingModule,
        CommonModule,
        FormsModule,
        SharedComponentsModule,
    ]
})
export class ProfessionalModule {

}