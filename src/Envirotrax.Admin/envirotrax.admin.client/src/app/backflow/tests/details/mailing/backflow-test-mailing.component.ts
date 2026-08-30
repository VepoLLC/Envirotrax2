import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';
import { State } from '../../../../shared/models/lookup/state';

@Component({
    selector: 'vp-backflow-test-mailing',
    templateUrl: './backflow-test-mailing.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestMailingComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;
    @Input() public stateOptions: InputOption<State>[] = [];

    @Output() public openSite: EventEmitter<void> = new EventEmitter<void>();

    public mailingStateId: string = '';

    public ngOnInit(): void {
        this.mailingStateId = this.test.mailingState?.id == null ? '' : String(this.test.mailingState.id);
    }

    public onMailingStateChange(value: string): void {
        this.test.mailingState = this.stateOptions.find(option => option.id === value)?.data;
    }
}
