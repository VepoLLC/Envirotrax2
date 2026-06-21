import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsFogRoutingModule } from "./professionals-fog-routing.module";
import { ProfessionalFogInspectionListComponent } from "./inspections/list/professional-fog-inspection-list.component";
import { FogInspectionViewComponent } from "./inspections/view/fog-inspection-view.component";

@NgModule({
    declarations: [
        ProfessionalFogInspectionListComponent,
        FogInspectionViewComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsFogRoutingModule
    ]
})
export class ProfessionalsFogModule {}
