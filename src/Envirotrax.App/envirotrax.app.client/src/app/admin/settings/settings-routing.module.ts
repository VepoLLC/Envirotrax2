import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GeneralSettingsComponent } from "./general/general-settings.component";
import { CsiSettingsComponent } from "./csi-settings/csi-settings.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";
import { BackflowTestingSettings } from "./backflow-testing-settings/backflow-testing-settings";
import { CsiLetterMessageSettingsComponent } from "./csi-letter-message-settings/csi-letter-message-settings.component";
import { BackflowLetterMessageSettingsComponent } from "./backflow-letter-message-settings/backflow-letter-message-settings.component";

const routes: Routes = [
    {
        path: '',
        redirectTo: 'general',
        pathMatch: 'full'
    },
    {
        path: 'general',
        title: 'General Settings & Fees',
        component: GeneralSettingsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Settings,
                    action: PermissionAction.CanView
                }
            ]
        }
    },
            {
                path: 'csi-settings',
                title: 'CSI Settings',
                component: CsiSettingsComponent
            },
            {
                path: 'backflow-testing-settings',
                title: 'Backflow Testing Settings',
                component: BackflowTestingSettings
            },
            {
                path: 'csi-letter-message-settings',
                title: 'CSI Letter Message Setup',
                component: CsiLetterMessageSettingsComponent
            },
            {
                path: 'backflow-letter-message-settings',
                title: 'Backflow Letter Message Setup',
                component: BackflowLetterMessageSettingsComponent
            }

];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class SettingsRoutingModule {

}
