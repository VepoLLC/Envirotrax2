import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsCsiRoutingModule } from "./professionals-csi-routing.module";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectionViewComponent } from "./inspections/view/csi-inspection-view.component";

@NgModule({
    declarations: [
        CsiSubmissionPropertySearchComponent,
        CsiSubmissionCreateComponent,
        CsiInspectionListComponent,
        CsiInspectionViewComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsCsiRoutingModule
    ]
})
export class ProfessionalsCsiModule {}
