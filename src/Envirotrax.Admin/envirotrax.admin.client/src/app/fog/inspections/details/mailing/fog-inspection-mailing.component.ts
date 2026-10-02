import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Optional, Output, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogInspection } from '../../../../shared/models/fog/fog-inspection';
import { State } from '../../../../shared/models/lookup/state';

@Component({
    selector: 'vp-fog-inspection-mailing',
    templateUrl: './fog-inspection-mailing.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogInspectionMailingComponent implements OnInit {
    @Input() public inspection: FogInspection = {};
    @Input() public form?: NgForm;
    @Input() public stateOptions: InputOption<State>[] = [];

    @Output() public openSite: EventEmitter<void> = new EventEmitter<void>();

    public mailingStateId: string = '';

    public ngOnInit(): void {
        this.mailingStateId = this.inspection.mailingState?.id == null ? '' : String(this.inspection.mailingState.id);
    }

    public onMailingStateChange(value: string): void {
        this.inspection.mailingState = this.stateOptions.find(option => option.id === value)?.data;
    }
}
