import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../../shared/components/shared.components.module";
import { ProfessionalsFogTransportationRoutingModule } from "./professionals-fog-transportation-routing.module";
import { ProfessionalFogVehicleListComponent } from "./vehicles/list/professional-fog-vehicle-list.component";
import { EditFogVehicleComponent } from "./vehicles/edit/edit-fog-vehicle.component";

@NgModule({
    declarations: [
        ProfessionalFogVehicleListComponent,
        EditFogVehicleComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsFogTransportationRoutingModule
    ]
})
export class ProfessionalsFogTransportationModule {}
