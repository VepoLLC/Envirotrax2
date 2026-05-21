import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsBackflowRoutingModule } from "./professionals-backflow-routing.module";
import { GaugeListComponent } from "./gauges/gauge-list.component";
import { EditGaugeComponent } from "./gauges/edit/edit-gauge.component";
import { ProfessionalBackflowTestListComponent } from "./tests/professional-backflow-test-list.component";

@NgModule({
    declarations: [GaugeListComponent, EditGaugeComponent, ProfessionalBackflowTestListComponent],
    imports: [CommonModule, FormsModule, SharedComponentsModule, ProfessionalsBackflowRoutingModule]
})
export class ProfessionalsBackflowModule {}
