import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { CsiSettingsRoutingModule } from "./csi-settings-routing.module";
import { CsiSettingsComponent } from "./general/csi-settings.component";

@NgModule({
    declarations: [
        CsiSettingsComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        CsiSettingsRoutingModule
    ]
})
export class CsiSettingsModule {

}
