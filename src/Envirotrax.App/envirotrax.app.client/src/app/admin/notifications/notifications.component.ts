import { Component } from "@angular/core";

interface NotificationTabDefinition {
    path: string;
    title: string;
    iconCss: string;
}

@Component({
    standalone: false,
    templateUrl: './notifications.component.html'
})
export class NotificationsComponent {
    public readonly tabs: NotificationTabDefinition[] = [
        { path: 'immediate', title: 'Immediate', iconCss: 'fa-regular fa-paper-plane' },
        { path: 'end-of-day', title: 'End of Day', iconCss: 'fa-regular fa-calendar-day' },
        { path: 'end-of-week', title: 'End of Week', iconCss: 'fa-regular fa-calendar-week' },
        { path: 'end-of-month', title: 'End of Month', iconCss: 'fa-regular fa-calendar-days' },
        { path: 'settings', title: 'Notification Settings', iconCss: 'fa-regular fa-gear' }
    ];
}
