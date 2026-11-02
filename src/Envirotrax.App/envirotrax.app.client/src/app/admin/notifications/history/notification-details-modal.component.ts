import { Component } from "@angular/core";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { Notification } from "../../../shared/models/notifications/notification";
import { NotificationOptionsService } from "../../../shared/services/notifications/notification-options.service";

@Component({
    standalone: false,
    templateUrl: './notification-details-modal.component.html'
})
export class NotificationDetailsModalComponent {
    public readonly details: NotificationDetailsVm;

    constructor(
        private readonly _modalReference: ModalReference<Notification>,
        options: NotificationOptionsService
    ) {
        const notification = this._modalReference.config.model!;

        this.details = {
            description: notification.description ?? '',
            userAccount: notification.user?.emailAddress ?? '',
            backflowTestType: options.getReasonForTestText(notification.reasonForTest),
            propertyType: options.getPropertyTypeText(notification),
            filters: options.getFiltersText(notification),
            hazardTypes: options.getHazardTypesText(notification)
        };
    }

    public close(): void {
        this._modalReference.cancel();
    }
}

interface NotificationDetailsVm {
    description: string;
    userAccount: string;
    backflowTestType: string;
    propertyType: string;
    filters: string;
    hazardTypes: string;
}
