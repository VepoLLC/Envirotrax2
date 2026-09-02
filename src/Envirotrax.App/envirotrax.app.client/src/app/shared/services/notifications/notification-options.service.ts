import { Injectable } from "@angular/core";
import { InputOption } from "@envirotrax/common-ui";
import { NotificationDeliveryType } from "../../enums/notification-delivery-type.enum";
import { NotificationInterval } from "../../enums/notification-interval.enum";
import { BackflowTestOptionsService } from "../backflow/backflow-test-options.service";

@Injectable({
    providedIn: 'root'
})
export class NotificationOptionsService {
    public readonly reasonForTestOptions: InputOption[];

    public readonly intervalOptions: InputOption[] = [
        { id: NotificationInterval.Immediate, text: 'Immediate' },
        { id: NotificationInterval.EndOfDay, text: 'End of Day' },
        { id: NotificationInterval.EndOfWeek, text: 'End of Week' },
        { id: NotificationInterval.EndOfMonth, text: 'End of Month' }
    ];

    public readonly deliveryTypeOptions: InputOption[] = [
        { id: NotificationDeliveryType.Email, text: 'Email' }
    ];

    public readonly colorOptions: string[] = [
        '#ffffff', '#e0e0e0', '#d0d0d0',
        '#ff0000', '#ff33cc', '#cc33ff',
        '#0000ff', '#00ccff', '#00ffcc',
        '#00ff00', '#ffff00', '#ff9900'
    ];

    constructor(backflowOptions: BackflowTestOptionsService) {
        this.reasonForTestOptions = [
            { id: null, text: 'Any' },
            ...backflowOptions.reasonOptions
        ];
    }

    public getReasonForTestText(reasonForTest?: number | null): string {
        const option = this.reasonForTestOptions.find(o => o.id === (reasonForTest ?? null));
        return option?.text ?? '';
    }

    public getIntervalText(interval?: NotificationInterval): string {
        const option = this.intervalOptions.find(o => o.id === interval);
        return option?.text ?? '';
    }

    public getDeliveryTypeText(deliveryType?: NotificationDeliveryType): string {
        const option = this.deliveryTypeOptions.find(o => o.id === deliveryType);
        return option?.text ?? '';
    }
}
