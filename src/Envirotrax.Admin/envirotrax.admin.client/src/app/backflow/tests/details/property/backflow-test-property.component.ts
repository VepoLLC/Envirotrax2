import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';
import { State } from '../../../../shared/models/lookup/state';
import { PropertyType } from '../../../../shared/models/sites/site';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';

@Component({
    selector: 'vp-backflow-test-property',
    templateUrl: './backflow-test-property.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestPropertyComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public stateOptions: InputOption<State>[] = [];

    @Output() public openSite: EventEmitter<void> = new EventEmitter<void>();

    public propertyTypeId: string = '';
    public propertyStateId: string = '';

    public readonly propertyTypeOptions: InputOption[];

    constructor(private readonly _options: BackflowTestOptionsService) {
        this.propertyTypeOptions = this._options.propertyTypeOptions;
    }

    public ngOnInit(): void {
        this.propertyTypeId = String(this.test.propertyType ?? PropertyType.Residential);
        this.propertyStateId = this.test.propertyState?.id == null ? '' : String(this.test.propertyState.id);
    }

    public onPropertyTypeChange(value: string): void {
        this.test.propertyType = Number(value) as PropertyType;
    }

    public onPropertyStateChange(value: string): void {
        this.test.propertyState = this.stateOptions.find(option => option.id === value)?.data;
    }
}
