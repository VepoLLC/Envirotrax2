import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { FogRoutingModule } from "./fog-routing.module";
import { FogInspectorListComponent } from "./inspectors/list/fog-inspector-list.component";
import { FogInspectorDetailsComponent } from "./inspectors/details/fog-inspector-details.component";
import { FogInspectorWaterSuppliersComponent } from "./inspectors/details/water-suppliers/list/fog-inspector-water-suppliers.component";
import { FogInspectorUsersComponent } from "./inspectors/details/users/list/fog-inspector-users.component";
import { FogInspectorLicenseInsuranceComponent } from "./inspectors/details/license-insurance/list/fog-inspector-license-insurance.component";
import { FogInspectionListComponent } from "./inspections/list/fog-inspection-list.component";
import { ProfessionalModule } from "../professionals/professional.module";

@NgModule({
    declarations: [
        FogInspectorListComponent,
        FogInspectorDetailsComponent,
        FogInspectorWaterSuppliersComponent,
        FogInspectorUsersComponent,
        FogInspectorLicenseInsuranceComponent,
        FogInspectionListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        FogRoutingModule,
        ProfessionalModule
    ]
})
export class FogModule {

}
