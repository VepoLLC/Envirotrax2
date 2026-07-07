import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { SettingsRoutingModule } from "./settings-routing.module";
import { GeneralSettingsComponent } from "./general/general-settings.component";
import { CsiSettingsComponent } from "./csi-settings/csi-settings.component";
import { BackflowTestingSettings } from './backflow-testing-settings/backflow-testing-settings';
import { CsiLetterMessageSettingsComponent } from './csi-letter-message-settings/csi-letter-message-settings.component';
import { BackflowLetterMessageSettingsComponent } from './backflow-letter-message-settings/backflow-letter-message-settings.component';
import { BackflowRenewalRequirementsComponent } from './backflow-renewal-requirements/backflow-renewal-requirements.component';
import { EditBackflowRenewalRequirementComponent } from './backflow-renewal-requirements/edit/edit-backflow-renewal-requirement.component';

@NgModule({
    declarations: [
        GeneralSettingsComponent,
        CsiSettingsComponent,
        BackflowTestingSettings,
        CsiLetterMessageSettingsComponent,
        BackflowLetterMessageSettingsComponent,
        BackflowRenewalRequirementsComponent,
        EditBackflowRenewalRequirementComponent
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
