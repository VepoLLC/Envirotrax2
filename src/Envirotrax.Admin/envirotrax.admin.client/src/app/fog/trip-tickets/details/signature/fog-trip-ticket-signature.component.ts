import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
    selector: 'vp-fog-trip-ticket-signature',
    templateUrl: './fog-trip-ticket-signature.component.html',
    imports: [CommonModule]
})
export class FogTripTicketSignatureComponent {
    @Input() public label: string = '';
    @Input() public url?: string;
}
