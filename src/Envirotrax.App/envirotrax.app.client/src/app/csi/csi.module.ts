import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { CsiRoutingModule } from "./csi-routing.module";
import { CsiInspectionListComponent } from "./inspections/list/csi-inspection-list.component";
import { CsiInspectorListComponent } from "./inspectors/list/csi-inspector-list.component";

@NgModule({          
    declarations: [
        CsiInspectionListComponent,
        CsiInspectorListComponent
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
