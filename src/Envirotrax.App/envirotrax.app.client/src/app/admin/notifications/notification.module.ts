import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { SharedComponentsModule } from "../../shared/components/shared.components.module";
import { NotificationsComponent } from "./notifications.component";
import { NotificationHistoryComponent } from "./history/notification-history.component";
import { NotificationDetailsModalComponent } from "./history/notification-details-modal.component";
import { NotificationSettingListComponent } from "./list/notification-setting-list.component";
import { EditNotificationSettingComponent } from "./edit/edit-notification-setting.component";
import { NotificationRoutingModule } from "./notification-routing.module";

@NgModule({
    declarations: [
        NotificationsComponent,
        NotificationHistoryComponent,
        NotificationDetailsModalComponent,
        NotificationSettingListComponent,
        EditNotificationSettingComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        SharedComponentsModule,
        NotificationRoutingModule
    ]
})
export class NotificationModule {

}
