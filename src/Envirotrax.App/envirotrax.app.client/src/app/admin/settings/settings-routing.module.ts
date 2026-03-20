import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GeneralSettingsComponent } from "./general/general-settings.component";
import { CsiSettingsComponent } from "./csi-settings/csi-settings.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";

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
