import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { FogTripTicketSignatureComponent } from '../signature/fog-trip-ticket-signature.component';

/**
 * The disposal site block is a snapshot taken when the ticket was submitted, so it is read-only. The contact name
 * and delivery date were typed on the submission form and stay editable, like the generator fields.
 */
@Component({
    selector: 'vp-fog-trip-ticket-receiver',
    templateUrl: './fog-trip-ticket-receiver.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule, FogTripTicketSignatureComponent],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogTripTicketReceiverComponent implements OnChanges {
    @Input() public ticket: FogTripTicket = {};
    @Input() public form?: NgForm;

    public cityStateZip: string = '';

    public ngOnChanges(): void {
        const ticket = this.ticket;

        this.cityStateZip = [ticket.receiverCity ? `${ticket.receiverCity},` : '', ticket.receiverState, ticket.receiverZip]
            .filter(part => part)
            .join(' ');
    }
}
