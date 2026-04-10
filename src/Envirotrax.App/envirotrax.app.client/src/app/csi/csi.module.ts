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
import { CsiInspectorSubAccountsComponent } from "./inspectors/details/sub-accounts/csi-inspector-sub-accounts.component";
import { CsiInspectorLicenseInsuranceComponent } from "./inspectors/details/license-insurance/csi-inspector-license-insurance.component";

@NgModule({
    declarations: [
        CsiInspectionListComponent,
        CsiInspectorListComponent,
        CsiInspectorDetailsComponent,
        CsiInspectorWaterSuppliersComponent,
        CsiInspectorSubAccountsComponent,
        CsiInspectorLicenseInsuranceComponent
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
