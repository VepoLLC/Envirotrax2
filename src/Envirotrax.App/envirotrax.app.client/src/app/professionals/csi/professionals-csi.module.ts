import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { ProfessionalsCsiRoutingModule } from "./professionals-csi-routing.module";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";

@NgModule({
    declarations: [
        CsiSubmissionPropertySearchComponent,
        CsiSubmissionCreateComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        ProfessionalsCsiRoutingModule
    ]
})
export class ProfessionalsCsiModule {}
