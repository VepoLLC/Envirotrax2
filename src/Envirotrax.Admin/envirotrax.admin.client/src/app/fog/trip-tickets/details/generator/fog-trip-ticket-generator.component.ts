import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Optional, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { InterceptorType } from '../../../../shared/models/fog/fog-inspection';
import { FogTripTicket } from '../../../../shared/models/fog/fog-trip-ticket';
import { State } from '../../../../shared/models/lookup/state';
import { PropertyType } from '../../../../shared/models/sites/site';
import { FogInspectionOptionsService } from '../../../../shared/services/fog/fog-inspection-options.service';
import { FogTripTicketSignatureComponent } from '../signature/fog-trip-ticket-signature.component';

@Component({
    selector: 'vp-fog-trip-ticket-generator',
    templateUrl: './fog-trip-ticket-generator.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule, FogTripTicketSignatureComponent],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogTripTicketGeneratorComponent implements OnChanges {
    @Input() public ticket: FogTripTicket = {};
    @Input() public form?: NgForm;
    @Input() public stateOptions: InputOption<State>[] = [];

    @Output() public openSite: EventEmitter<void> = new EventEmitter<void>();

    public propertyTypeId: string = '';
    public propertyStateId: string = '';
    public isOtherInterceptorType: boolean = false;

    public readonly propertyTypeOptions: InputOption[] = [
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public readonly interceptorTypeOptions: InputOption[];
    public readonly capacityTypeOptions: InputOption[];

    constructor(private readonly _options: FogInspectionOptionsService) {
        this.interceptorTypeOptions = this._options.tripTicketInterceptorTypeOptions;
        this.capacityTypeOptions = this._options.capacityTypeOptions;
    }

    // Also runs when the parent reloads the ticket after a save.
    public ngOnChanges(): void {
        this.propertyTypeId = String(this.ticket.propertyType ?? PropertyType.Residential);
        this.propertyStateId = this.ticket.propertyState?.id == null ? '' : String(this.ticket.propertyState.id);
        this.onInterceptorTypeChange();
    }

    public onPropertyTypeChange(value: string): void {
        this.ticket.propertyType = Number(value) as PropertyType;
    }

    public onPropertyStateChange(value: string): void {
        this.ticket.propertyState = this.stateOptions.find(option => option.id === value)?.data;
    }

    public onInterceptorTypeChange(): void {
        this.isOtherInterceptorType = this.ticket.interceptorType === InterceptorType.Other;
    }
}
