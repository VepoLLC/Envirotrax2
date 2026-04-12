import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../shared/components/shared.components.module";
import { FogRoutingModule } from "./fog-routing.module";
import { FogInspectorListComponent } from "./inspectors/list/fog-inspector-list.component";

@NgModule({
    declarations: [
        FogInspectorListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        FogRoutingModule
    ]
})
export class FogModule {

}
