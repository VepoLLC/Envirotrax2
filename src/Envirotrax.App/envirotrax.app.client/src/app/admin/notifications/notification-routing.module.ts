import { RouterModule, Routes } from "@angular/router";
import { NgModule } from "@angular/core";
import { NotificationsComponent } from "./notifications.component";
import { NotificationHistoryComponent } from "./history/notification-history.component";
import { NotificationSettingListComponent } from "./list/notification-setting-list.component";
import { PermissionGuard } from "../../shared/guards/permission.guard";
import { PermissionAction, PermissionType } from "../../shared/models/permission-type";
import { NotificationInterval } from "../../shared/enums/notification-interval.enum";

const routes: Routes = [
    {
        path: '',
        component: NotificationsComponent,
        canActivate: [PermissionGuard],
        data: {
            permissions: [
                {
                    type: PermissionType.Notifications,
                    action: PermissionAction.CanView
                }
            ]
        },
        children: [
            { path: '', pathMatch: 'full', redirectTo: 'immediate' },
            { path: 'immediate', title: 'Immediate Notifications', component: NotificationHistoryComponent, data: { interval: NotificationInterval.Immediate } },
            { path: 'end-of-day', title: 'End of Day Notifications', component: NotificationHistoryComponent, data: { interval: NotificationInterval.EndOfDay } },
            { path: 'end-of-week', title: 'End of Week Notifications', component: NotificationHistoryComponent, data: { interval: NotificationInterval.EndOfWeek } },
            { path: 'end-of-month', title: 'End of Month Notifications', component: NotificationHistoryComponent, data: { interval: NotificationInterval.EndOfMonth } },
            { path: 'settings', title: 'Notification Settings', component: NotificationSettingListComponent }
        ]
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
