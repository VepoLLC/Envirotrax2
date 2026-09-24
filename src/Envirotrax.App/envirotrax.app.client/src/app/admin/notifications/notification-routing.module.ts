import { RouterModule, Routes } from "@angular/router";
import { NgModule } from "@angular/core";
import { NotificationSettingListComponent } from "./list/notification-setting-list.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";

const routes: Routes = [
    {
        path: '',
        title: 'Notifications',
        component: NotificationSettingListComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Notifications,
                    action: PermissionAction.CanView
                }
            ]
        }
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
export class NotificationRoutingModule {

}
