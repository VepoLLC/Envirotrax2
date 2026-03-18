import { NgModule } from "@angular/core";
import { ProfessionalRoutingModule } from "./professional-routing.module";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { WaterSuppliersComponent } from "./water-supppliers/water-suppliers.component";
import { WaterSupplierRegistrationComponent } from "./water-supppliers/registration/water-supplier-registration.component";

@NgModule({
    declarations: [
        WaterSuppliersComponent,
        WaterSupplierRegistrationComponent
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