import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { CsiRoutingModule } from "./csi-routing.module";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectorListComponent } from "./inspectors/list/csi-inspector-list.component";
import { CsiInspectorDetailsComponent } from "./inspectors/details/csi-inspector-details.component";
import { CsiInspectorWaterSuppliersComponent } from "./inspectors/details/water-suppliers/csi-inspector-water-suppliers.component";
import { CsiInspectorUsersComponent } from "./inspectors/details/users/csi-inspector-users.component";
import { CsiInspectorLicenseInsuranceComponent } from "./inspectors/details/license-insurance/csi-inspector-license-insurance.component";
import { CsiSubmissionPropertySearchComponent } from "./inspections/create/csi-submission-property-search.component";
import { CsiSubmissionCreateComponent } from "./inspections/create/csi-submission-create.component";

@NgModule({
    declarations: [
        CsiInspectionListComponent,
        CsiInspectorListComponent,
        CsiInspectorDetailsComponent,
        CsiInspectorWaterSuppliersComponent,
        CsiInspectorSubAccountsComponent,
        CsiInspectorLicenseInsuranceComponent,
        CsiSubmissionPropertySearchComponent,
        CsiSubmissionCreateComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        CsiRoutingModule
    ]
})
export class CsiModule {

}
