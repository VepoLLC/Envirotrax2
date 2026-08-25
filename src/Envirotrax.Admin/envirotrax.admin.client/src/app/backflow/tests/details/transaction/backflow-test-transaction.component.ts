import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';

@Component({
    selector: 'vp-backflow-test-transaction',
    templateUrl: './backflow-test-transaction.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestTransactionComponent {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
}
