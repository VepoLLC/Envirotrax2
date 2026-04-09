import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { SettingsRoutingModule } from "./settings-routing.module";
import { GeneralSettingsComponent } from "./general/general-settings.component";
import { CsiSettingsComponent } from "./csi-settings/csi-settings.component";
import { BackflowTestingSettings } from './backflow-testing-settings/backflow-testing-settings';

@NgModule({
    declarations: [
        GeneralSettingsComponent,
        CsiSettingsComponent,
        BackflowTestingSettings
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        SettingsRoutingModule
    ]
})
export class SettingsModule {

}
