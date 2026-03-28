import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { CsiRoutingModule } from "./csi-routing.module";
import { CsiSearchComponent } from "./search/csi-search.component";
import { InspectorManagementComponent } from './inspector-management/inspector-management.component';

@NgModule({
    declarations: [
        CsiSearchComponent,
        InspectorManagementComponent
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
