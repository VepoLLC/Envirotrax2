import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import {
    BackflowTestDetails,
    backflowDeviceTypeDescriptions
} from '../../../../shared/models/backflow/backflow-test';
import { BackflowAssemblyVisibility, buildAssemblyVisibility } from '../backflow-test-visibility';

@Component({
    selector: 'vp-backflow-test-readings',
    templateUrl: './backflow-test-readings.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestReadingsComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public idPrefix: string = '';
    @Input() public visibility: BackflowAssemblyVisibility = buildAssemblyVisibility(undefined);

    public initialTestHeader: string = 'Initial Test';
    public pvbHeader: string = 'Pressure Vacuum Breaker';

    public ngOnInit(): void {
        const description = this.test.deviceType
            ? backflowDeviceTypeDescriptions[this.test.deviceType]
            : undefined;

        this.initialTestHeader = description ? `Initial Test - ${description}` : 'Initial Test';
        this.pvbHeader = description ?? 'Pressure Vacuum Breaker';
    }
}
