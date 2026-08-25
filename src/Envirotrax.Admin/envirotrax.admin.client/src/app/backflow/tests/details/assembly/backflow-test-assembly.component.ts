import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';
import { BackflowTestOptionsService } from '../../../../shared/services/backflow/backflow-test-options.service';
import { BackflowAssemblyVisibility, buildAssemblyVisibility } from '../backflow-test-visibility';

const HazardTypeOther = 'Other';

@Component({
    selector: 'vp-backflow-test-assembly',
    templateUrl: './backflow-test-assembly.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestAssemblyComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public visibility: BackflowAssemblyVisibility = buildAssemblyVisibility(undefined);

    @Output() public deviceTypeChange: EventEmitter<void> = new EventEmitter<void>();

    public isHazardTypeOther: boolean = false;

    public readonly deviceTypeOptions: InputOption[];
    public readonly manufacturerOptions: InputOption[];
    public readonly sizeOptions: InputOption[];
    public readonly hazardTypeOptions: InputOption[];

    constructor(private readonly _options: BackflowTestOptionsService) {
        this.deviceTypeOptions = this._options.deviceTypeFormOptions;
        this.manufacturerOptions = this._options.manufacturerOptions;
        this.sizeOptions = this._options.sizeOptions;
        this.hazardTypeOptions = this._options.hazardTypeFormOptions;
    }

    public ngOnInit(): void {
        this.isHazardTypeOther = this.test.hazardType === HazardTypeOther;
    }

    public onDeviceTypeChange(): void {
        this.deviceTypeChange.emit();
    }

    public onHazardTypeChange(): void {
        this.isHazardTypeOther = this.test.hazardType === HazardTypeOther;
    }
}
