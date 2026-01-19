import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { SettingsRoutingModule } from "./settings-routing.module";
import { GeneralSettingsComponent } from "./general/general-settings.component";

@NgModule({
    declarations: [
        GeneralSettingsComponent
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
