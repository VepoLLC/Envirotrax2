import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SignaturePadComponent } from "@almothafar/angular-signature-pad";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsFogRoutingModule } from "./professionals-fog-routing.module";
import { ProfessionalFogInspectionListComponent } from "./inspections/list/professional-fog-inspection-list.component";
import { FogInspectionViewComponent } from "./inspections/view/fog-inspection-view.component";
import { ProfessionalFogSubmissionPropertySearchComponent } from "./inspections/create/professional-fog-submission-property-search.component";
import { ProfessionalFogSubmissionCreateComponent } from "./inspections/create/professional-fog-submission-create.component";
import { FogSignaturePadModalComponent } from "./inspections/create/fog-signature-pad-modal.component";

@NgModule({
    declarations: [
        ProfessionalFogInspectionListComponent,
        FogInspectionViewComponent,
        ProfessionalFogSubmissionPropertySearchComponent,
        ProfessionalFogSubmissionCreateComponent,
        FogSignaturePadModalComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SignaturePadComponent,
        SharedComponentsModule,
        ProfessionalsFogRoutingModule
    ]
})
export class ProfessionalsFogModule {}
