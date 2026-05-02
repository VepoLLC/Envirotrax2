import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { CsiRoutingModule } from "./csi-routing.module";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectorListComponent } from "./inspectors/list/csi-inspector-list.component";
import { CsiInspectorDetailsComponent } from "./inspectors/details/csi-inspector-details.component";
import { CsiInspectorWaterSuppliersComponent } from "./inspectors/details/water-suppliers/list/csi-inspector-water-suppliers.component";
import { CsiInspectorUsersComponent } from "./inspectors/details/users/csi-inspector-users.component";
import { CsiInspectorLicenseInsuranceComponent } from "./inspectors/details/license-insurance/list/csi-inspector-license-insurance.component";
import { CsiInspectorAddEditLicenseComponent } from "./inspectors/details/license-insurance/edit/add-edit-csi-inspector-license.component";
import { CsiInspectorAddEditInsuranceComponent } from "./inspectors/details/license-insurance/edit/add-edit-csi-inspector-insurance.component";
import { EditCsiInspectorWaterSupplierComponent } from "./inspectors/details/water-suppliers/edit/edit-csi-inspector-water-supplier.component";
import { ProfessionalModule } from "../professionals/professional.module";

@NgModule({
    declarations: [
        CsiInspectionListComponent,
        CsiInspectorListComponent,
        CsiInspectorDetailsComponent,
        CsiInspectorWaterSuppliersComponent,
        CsiInspectorUsersComponent,
        CsiInspectorLicenseInsuranceComponent,
        CsiInspectorAddEditLicenseComponent,
        CsiInspectorAddEditInsuranceComponent,
        EditCsiInspectorWaterSupplierComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        CsiRoutingModule,
        ProfessionalModule,
    ]
})
export class CsiModule {

}
