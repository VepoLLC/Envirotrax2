import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Optional, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { State } from '../../../../shared/models/lookup/state';
import { PropertyType } from '../../../../shared/models/sites/site';

@Component({
    selector: 'vp-fog-inspection-property',
    templateUrl: './fog-inspection-property.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogInspectionPropertyComponent implements OnInit {
    @Input() public inspection: FogInspection = {};
    @Input() public form?: NgForm;
    @Input() public stateOptions: InputOption<State>[] = [];

    @Output() public openSite: EventEmitter<void> = new EventEmitter<void>();

    public propertyTypeId: string = '';
    public propertyStateId: string = '';

    public readonly propertyTypeOptions: InputOption[] = [
        { id: String(PropertyType.Residential), text: 'Residential' },
        { id: String(PropertyType.Commercial), text: 'Commercial' }
    ];

    public ngOnInit(): void {
        this.propertyTypeId = String(this.inspection.propertyType ?? PropertyType.Residential);
        this.propertyStateId = this.inspection.propertyState?.id == null ? '' : String(this.inspection.propertyState.id);
    }

    public onPropertyTypeChange(value: string): void {
        this.inspection.propertyType = Number(value) as PropertyType;
    }

    public onPropertyStateChange(value: string): void {
        this.inspection.propertyState = this.stateOptions.find(option => option.id === value)?.data;
    }
}
