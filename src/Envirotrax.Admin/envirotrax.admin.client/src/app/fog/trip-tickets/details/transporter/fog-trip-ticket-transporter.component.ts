import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges } from '@angular/core';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { InterceptorCapacityType, interceptorCapacityTypeLabels } from '../../../../shared/models/fog/fog-inspection';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { FogTripTicketSignatureComponent } from '../signature/fog-trip-ticket-signature.component';

/** Read-only: the transporter and vehicle are snapshots taken when the ticket was submitted. */
@Component({
    selector: 'vp-fog-trip-ticket-transporter',
    templateUrl: './fog-trip-ticket-transporter.component.html',
    imports: [CommonModule, SharedComponentsModule, FogTripTicketSignatureComponent]
})
export class FogTripTicketTransporterComponent implements OnChanges {
    @Input() public ticket: FogTripTicket = {};

    public cityStateZip: string = '';
    public vehicleDescription: string = '';
    public vehicleCapacityDescription: string = '';

    public ngOnChanges(): void {
        const ticket = this.ticket;

        this.cityStateZip = [ticket.transporterCity ? `${ticket.transporterCity},` : '', ticket.transporterState, ticket.transporterZip]
            .filter(part => part)
            .join(' ');

        this.vehicleDescription = [ticket.vehicleYear || '', ticket.vehicleManufacturer]
            .filter(part => part)
            .join(' ');

        this.vehicleCapacityDescription = ticket.vehicleCapacity
            ? `${ticket.vehicleCapacity} ${interceptorCapacityTypeLabels[ticket.vehicleCapacityType ?? InterceptorCapacityType.Gallons]}`
            : '';
    }
}
