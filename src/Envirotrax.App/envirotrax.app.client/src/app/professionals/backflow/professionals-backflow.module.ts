import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { BackflowTestDetailsSectionsModule } from "../../backflow/tests/details/backflow-test-details-sections.module";
import { ProfessionalsBackflowRoutingModule } from "./professionals-backflow-routing.module";
import { GaugeListComponent } from "./gauges/gauge-list.component";
import { EditGaugeComponent } from "./gauges/edit/edit-gauge.component";
import { ProfessionalBackflowTestListComponent } from "./tests/professional-backflow-test-list.component";
import { ProfessionalBackflowTestDetailsComponent } from "./tests/details/professional-backflow-test-details.component";
import { ProfessionalBackflowTestBpatInfoComponent } from "./tests/details/bpat-info/professional-backflow-test-bpat-info.component";
import { ProfessionalBackflowTestPropertyInfoComponent } from "./tests/details/property-info/professional-backflow-test-property-info.component";
import { ProfessionalBackflowTestMailingInfoComponent } from "./tests/details/mailing-info/professional-backflow-test-mailing-info.component";
import { ProfessionalBackflowTestBackflowInfoComponent } from "./tests/details/backflow-info/professional-backflow-test-backflow-info.component";
import { BackflowTestAssemblySearchComponent } from "./submit/backflow-test-assembly-search.component";
import { BackflowTestSubmitComponent } from "./submit/backflow-test-submit.component";
import { BackflowOutOfServiceRequestComponent } from "./tests/out-of-service/backflow-out-of-service-request.component";

@NgModule({
    declarations: [
        GaugeListComponent,
        EditGaugeComponent,
        ProfessionalBackflowTestListComponent,
        ProfessionalBackflowTestDetailsComponent,
        ProfessionalBackflowTestBpatInfoComponent,
        ProfessionalBackflowTestPropertyInfoComponent,
        ProfessionalBackflowTestMailingInfoComponent,
        ProfessionalBackflowTestBackflowInfoComponent,
        BackflowTestAssemblySearchComponent,
        BackflowTestSubmitComponent,
        BackflowOutOfServiceRequestComponent
    ],
    imports: [CommonModule, FormsModule, SharedComponentsModule, BackflowTestDetailsSectionsModule, ProfessionalsBackflowRoutingModule]
})
export class ProfessionalsBackflowModule {}
