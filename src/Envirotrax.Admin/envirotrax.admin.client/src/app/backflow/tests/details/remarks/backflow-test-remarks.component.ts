import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-remarks',
    templateUrl: './backflow-test-remarks.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestRemarksComponent {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
}
